// FILE: src/Infrastructure/Services/MockEmailSender.cs (MODIFIED)
using CRM_Vivid.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System; // Required for Guid
using Task = System.Threading.Tasks.Task;

namespace CRM_Vivid.Infrastructure.Services;

public class MockEmailSender : IEmailSender
{
  private readonly ILogger<MockEmailSender> _logger;

  public MockEmailSender(ILogger<MockEmailSender> logger)
  {
    _logger = logger;
  }

  // --- FIX: Implement the full interface signature (Phase 38) ---
  public Task SendEmailAsync(
      string to,
      string subject,
      string body,
      Guid? templateId = null, // NEW: Added optional parameter
      Guid? eventId = null)    // NEW: Added optional parameter
  {
    _logger.LogInformation("--------------------------------------------------");
    _logger.LogInformation("--> [MOCK EMAIL SENDER] Triggered");
    _logger.LogInformation($"--> To: {to}");
    _logger.LogInformation($"--> Subject: {subject}");
    _logger.LogInformation($"--> Body: {body}");

    // Log the new context for auditing purposes
    if (templateId.HasValue)
    {
      _logger.LogInformation($"--> Template ID: {templateId}");
    }
    if (eventId.HasValue)
    {
      _logger.LogInformation($"--> Event ID: {eventId}");
    }

    _logger.LogInformation("--------------------------------------------------");

    return Task.CompletedTask;
  }
}