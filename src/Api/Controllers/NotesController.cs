using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Notes.Queries;
// --- FIX 1: Correct Namespace Import (Matches your file tree) ---
using CRM_Vivid.Application.Notes.Commands;

namespace CRM_Vivid.Api.Controllers;

[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
  private readonly ISender _mediator;

  public NotesController(ISender mediator)
  {
    _mediator = mediator;
  }

  [HttpGet]
  public async Task<IEnumerable<NoteDto>> GetNotes([FromQuery] GetNotesQuery query)
  {
    return await _mediator.Send(query);
  }

  [HttpPost]
  // --- FIX 2: Return NoteDto, not Guid ---
  public async Task<ActionResult<NoteDto>> Create(CreateNoteCommand command)
  {
    return await _mediator.Send(command);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult> Update(Guid id, UpdateNoteCommand command)
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
    // --- FIX 3: Use Object Initializer Syntax ---
    await _mediator.Send(new DeleteNoteCommand { Id = id });
    return NoContent();
  }
}