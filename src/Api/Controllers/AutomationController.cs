using CRM_Vivid.Application.Automation.Commands;
using CRM_Vivid.Application.Common.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Vivid.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AutomationController : ControllerBase
{
  private readonly IBackgroundJobClient _jobClient;
  private readonly IEmailSender _emailSender;
  private readonly ISender _sender;

  public AutomationController(IBackgroundJobClient jobClient, IEmailSender emailSender, ISender sender)
  {
    _jobClient = jobClient;
    _emailSender = emailSender;
    _sender = sender;
  }

  [HttpPost("test-email")]
  public IActionResult SendTestEmail([FromQuery] string email)
  {
    // Fire-and-forget job
    var jobId = _jobClient.Enqueue(() => _emailSender.SendEmailAsync(email, "Proof of Life", "The Automation Engine is Online."));

    return Ok(new { Message = "Email job enqueued", JobId = jobId });
  }

  [HttpPost("schedule-email")]
  public async Task<IActionResult> ScheduleEmail([FromBody] ScheduleEmailCommand command)
  {
    await _sender.Send(command);
    return Ok(new { Message = "Email scheduled successfully." });
  }
}