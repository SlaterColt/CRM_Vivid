using CRM_Vivid.Application.Automation.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Vivid.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AutomationController : ControllerBase
{
  private readonly ISender _sender;

  public AutomationController(ISender sender)
  {
    _sender = sender;
  }

  // NOTE: SendTestEmail and ScheduleEmail logic has been removed/refactored
  // to avoid exposing IBackgroundJobClient directly and centralize scheduling via ISender.

  [HttpPost("schedule-followup")]
  [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> ScheduleFollowUp([FromBody] ScheduleFollowUpCommand command)
  {
    // The command handler validates the time/IDs and queues the job via Hangfire.
    var jobId = await _sender.Send(command);

    return Ok(new { Message = "Follow-up job scheduled successfully.", JobId = jobId });
  }

  // Placeholder for future general automation actions
  // [HttpPost("trigger-workflow")]
  // public IActionResult TriggerWorkflow(...) { ... }
}