// FILE: src/Infrastructure/Services/SendGridEmailSender.cs

using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities; // For EmailLog
using Microsoft.EntityFrameworkCore; // For querying Contacts
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Task = System.Threading.Tasks.Task;

namespace CRM_Vivid.Infrastructure.Services;

public class SendGridEmailSender : IEmailSender
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<SendGridEmailSender> _logger;
  private readonly IApplicationDbContext _context;

  public SendGridEmailSender(
      IConfiguration configuration,
      ILogger<SendGridEmailSender> logger,
      IApplicationDbContext context)
  {
    _configuration = configuration;
    _logger = logger;
    _context = context;
  }

  // --- FIX: IMPLEMENT INTERFACE METHOD WITH ALL NEW OPTIONAL PARAMETERS ---
  public async Task SendEmailAsync(
      string email,
      string subject,
      string htmlMessage,
      Guid? templateId = null, // NEW: Added optional parameter
      Guid? eventId = null)    // NEW: Added optional parameter
  {
    var apiKey = _configuration["SendGrid:ApiKey"];

    // 1. Prepare the Log Entry
    var emailLog = new EmailLog
    {
      Id = Guid.NewGuid(),
      To = email,
      Subject = subject,
      Body = htmlMessage,
      SentAt = DateTime.UtcNow,
      IsSuccess = false, // Default to false until proven otherwise

      // --- PHASE 38 FIX: ASSIGN TEMPLATE AND EVENT CONTEXT ---
      TemplateId = templateId,
      EventId = eventId
      // --------------------------------------------------------
    };

    // 2. Try to link to a Contact
    try
    {
      var contact = await _context.Contacts
          .FirstOrDefaultAsync(c => c.Email == email);

      if (contact != null)
      {
        emailLog.ContactId = contact.Id;
      }
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Failed to resolve Contact ID for email logging.");
    }

    // 3. Validation Check
    if (string.IsNullOrEmpty(apiKey))
    {
      var error = "SendGrid API Key is missing.";
      _logger.LogWarning("{Error} Email to {Email} was not sent.", error, email);

      emailLog.ErrorMessage = error;
      _context.EmailLogs.Add(emailLog);
      await _context.SaveChangesAsync(CancellationToken.None);
      return;
    }

    // 4. Attempt Send
    try
    {
      var client = new SendGridClient(apiKey);
      var fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@crmvivid.com";
      var fromName = _configuration["SendGrid:FromName"] ?? "CRM Vivid";

      var from = new EmailAddress(fromEmail, fromName);
      var to = new EmailAddress(email);
      var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlMessage, htmlMessage);

      var response = await client.SendEmailAsync(msg);

      if (response.IsSuccessStatusCode)
      {
        _logger.LogInformation("Email successfully queued/sent to {Email} via SendGrid.", email);
        emailLog.IsSuccess = true;
      }
      else
      {
        var errorMsg = $"SendGrid Error: {response.StatusCode}";
        _logger.LogError("Failed to send email to {Email}. {Status}", email, errorMsg);
        emailLog.ErrorMessage = errorMsg;
        emailLog.IsSuccess = false;
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception while sending email to {Email}", email);
      emailLog.ErrorMessage = ex.Message;
      emailLog.IsSuccess = false;

      // We rethrow if we want Hangfire to retry, 
      // BUT we must save the log first or we lose the record of the attempt.
    }

    // 5. Save Log
    _context.EmailLogs.Add(emailLog);
    await _context.SaveChangesAsync(CancellationToken.None);
  }
}