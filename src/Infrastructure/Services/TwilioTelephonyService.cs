using CRM_Vivid.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CRM_Vivid.Infrastructure.Services;

public class TwilioTelephonyService : ITelephonyService
{
  private readonly string _accountSid;
  private readonly string _authToken;
  private readonly string _twilioPhoneNumber;
  private readonly ILogger<TwilioTelephonyService> _logger;

  public TwilioTelephonyService(IConfiguration configuration, ILogger<TwilioTelephonyService> logger)
  {
    _logger = logger;

    _accountSid = configuration["Twilio:AccountSid"] ?? string.Empty;
    _authToken = configuration["Twilio:AuthToken"] ?? string.Empty;

    var rawPhone = configuration["Twilio:PhoneNumber"] ?? string.Empty;
    _twilioPhoneNumber = FormatNumber(rawPhone);

    if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken))
    {
      _logger.LogWarning("Twilio configuration is missing. SMS functionality will be disabled.");
    }
    else
    {
      try
      {
        TwilioClient.Init(_accountSid, _authToken);
        _logger.LogInformation("Twilio Client Initialized. Sending from: {Phone}", _twilioPhoneNumber);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to initialize Twilio Client.");
      }
    }
  }

  private async Task<bool> SendMessage(string toPhoneNumber, string body)
  {
    if (string.IsNullOrEmpty(_accountSid))
    {
      _logger.LogError("Attempted to send SMS but AccountSID is null.");
      return false;
    }

    if (_twilioPhoneNumber == "INVALID")
    {
      _logger.LogError("Twilio configuration error: The 'From' phone number is invalid.");
      return false;
    }

    // 1. Format the Destination Number
    string formattedTo = FormatNumber(toPhoneNumber);
    if (formattedTo == "INVALID")
    {
      _logger.LogError("SMS failed: Target number '{Phone}' is not a valid US/E.164 number.", toPhoneNumber);
      return false;
    }

    try
    {
      // 2. Send via Twilio
      var message = await MessageResource.CreateAsync(
          to: new PhoneNumber(formattedTo),
          from: new PhoneNumber(_twilioPhoneNumber),
          body: body
      );

      // 3. Check Result
      if (message.Status == MessageResource.StatusEnum.Queued || message.Status == MessageResource.StatusEnum.Sending)
      {
        _logger.LogInformation("SMS Queued via Twilio. SID: {Sid} | To: {To}", message.Sid, formattedTo);
        return true;
      }

      // 4. Log Specific Failure
      _logger.LogError("Twilio Rejected. Status: {Status} | Error Code: {Code} | Message: {Msg}",
          message.Status, message.ErrorCode, message.ErrorMessage);
      return false;
    }
    catch (Twilio.Exceptions.ApiException apiEx)
    {
      // Capture specific Twilio API errors (like Unverified Caller ID or Auth failures)
      _logger.LogError("Twilio API Error: {Msg} (Code: {Code})", apiEx.Message, apiEx.Code);
      return false;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected Exception sending SMS to {Phone}", formattedTo);
      return false;
    }
  }

  private string FormatNumber(string raw)
  {
    if (string.IsNullOrWhiteSpace(raw)) return "INVALID";

    string cleaned = raw.Trim();

    // If it starts with +, assume it is already E.164 (e.g. +14043901326)
    if (cleaned.StartsWith("+")) return cleaned;

    // Strip non-digits to handle (404) 555-0100
    var digits = new string(cleaned.Where(char.IsDigit).ToArray());

    // Standard US 10-digit -> +14045550100
    if (digits.Length == 10) return "+1" + digits;

    // US 11-digit with leading 1 -> +14045550100
    if (digits.Length == 11 && digits.StartsWith("1")) return "+" + digits;

    // Fallback: If we can't determine the format, return INVALID or let Twilio try if length is sufficient
    if (digits.Length > 7) return "+" + digits;

    return "INVALID";
  }

  public Task<bool> SendSmsAsync(string toPhoneNumber, string body)
  {
    return SendMessage(toPhoneNumber, body);
  }

  public Task<bool> SendCriticalAlertAsync(string toPhoneNumber, string alertMessage)
  {
    return SendMessage(toPhoneNumber, $"[CRITICAL ALERT]: {alertMessage}");
  }
}