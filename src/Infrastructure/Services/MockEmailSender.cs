using CRM_Vivid.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CRM_Vivid.Infrastructure.Services;

public class MockEmailSender : IEmailSender
{
  private readonly ILogger<MockEmailSender> _logger;

  public MockEmailSender(ILogger<MockEmailSender> logger)
  {
    _logger = logger;
  }

  public Task SendEmailAsync(string to, string subject, string body)
  {
    _logger.LogInformation("--------------------------------------------------");
    _logger.LogInformation("--> [MOCK EMAIL SENDER] Triggered");
    _logger.LogInformation($"--> To: {to}");
    _logger.LogInformation($"--> Subject: {subject}");
    _logger.LogInformation($"--> Body: {body}");
    _logger.LogInformation("--------------------------------------------------");

    return Task.CompletedTask;
  }
}