// FILE: src/Infrastructure/Services/TwilioTelephonyService.cs (MODIFIED)

using CRM_Vivid.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CRM_Vivid.Infrastructure.Services;

public class TwilioTelephonyService : ITelephonyService
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<TwilioTelephonyService> _logger;
  private readonly string _accountSid;
  private readonly string _authToken;
  private readonly string _twilioPhoneNumber;

  public TwilioTelephonyService(IConfiguration configuration, ILogger<TwilioTelephonyService> logger)
  {
    _configuration = configuration;
    _logger = logger;

    // Load credentials from configuration
    _accountSid = _configuration["Twilio:AccountSid"] ??
                  throw new ArgumentNullException("Twilio:AccountSid is missing.");
    _authToken = _configuration["Twilio:AuthToken"] ??
                 throw new ArgumentNullException("Twilio:AuthToken is missing.");
    // This is the number the user must own and submit via configuration (or later, the UI/DB)
    _twilioPhoneNumber = _configuration["Twilio:PhoneNumber"] ??
                         throw new ArgumentNullException("Twilio:PhoneNumber is missing.");

    // Initialize the Twilio client once per service instance
    TwilioClient.Init(_accountSid, _authToken);
  }

  private async Task<bool> SendMessage(string toPhoneNumber, string body)
  {
    if (string.IsNullOrWhiteSpace(toPhoneNumber))
    {
      _logger.LogWarning("Skipping SMS send: Recipient phone number is empty.");
      return false;
    }

    // CRITICAL: Twilio phone numbers must be in E.164 format (e.g., +15551234567)
    // We assume the input 'toPhoneNumber' is already valid, but proper validation is required in production.
    if (!toPhoneNumber.StartsWith("+"))
    {
      _logger.LogError("SMS failed: Phone number '{Phone}' is not in E.164 format (must start with '+').", toPhoneNumber);
      return false;
    }

    try
    {
      var message = await MessageResource.CreateAsync(
          to: new Twilio.Types.PhoneNumber(toPhoneNumber),
          from: new Twilio.Types.PhoneNumber(_twilioPhoneNumber),
          body: body
      );

      if (message.Status == MessageResource.StatusEnum.Queued || message.Status == MessageResource.StatusEnum.Sending)
      {
        _logger.LogInformation("SMS successfully queued via Twilio. SID: {Sid}", message.Sid);
        return true;
      }

      _logger.LogError("Twilio failed to send message. Status: {Status}, Error: {Error}", message.Status, message.ErrorMessage);
      return false;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception while sending SMS via Twilio to {Phone}", toPhoneNumber);
      return false;
    }
  }

  public Task<bool> SendSmsAsync(string toPhoneNumber, string body)
  {
    return SendMessage(toPhoneNumber, body);
  }

  public Task<bool> SendCriticalAlertAsync(string toPhoneNumber, string alertMessage)
  {
    // Prepend a critical identifier for easy filtering/logging
    string body = $"[VIVID ALERT] {alertMessage}";
    return SendMessage(toPhoneNumber, body);
  }
}