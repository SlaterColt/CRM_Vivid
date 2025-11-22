using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Tasks.Queries;
using CRM_Vivid.Application.Tasks.Commands;

namespace CRM_Vivid.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
  private readonly ISender _mediator;

  public TasksController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpGet]
  public async Task<IEnumerable<TaskDto>> GetTasks([FromQuery] GetTasksQuery query)
  {
    return await _mediator.Send(query);
  }

  [HttpPost]
  // --- FIX: Reverted to ActionResult<Guid> to match the Command ---
  public async Task<ActionResult<Guid>> Create(CreateTaskCommand command)
  {
    return await _mediator.Send(command);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult> Update(Guid id, UpdateTaskCommand command)
  {
    if (id != command.Id)
    {
      return BadRequest();
    }

    await _mediator.Send(command);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(Guid id)
  {
    await _mediator.Send(new DeleteTaskCommand { Id = id });
    return NoContent();
  }
}