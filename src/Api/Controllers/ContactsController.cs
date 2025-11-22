using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Contacts.Commands;
using CRM_Vivid.Application.Contacts.Queries;
using CRM_Vivid.Application.Features.Contacts.Commands; // Note: Check if this namespace is actually needed/correct based on your folder structure, usually it's just Application.Contacts.Commands
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Vivid.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
  private readonly IMediator _mediator;

  public ContactsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<Guid>> Create(CreateContactCommand command)
  {
    var contactId = await _mediator.Send(command);
    return Ok(contactId);
  }

  [HttpGet]
  public async Task<ActionResult<List<Contact>>> Get()
  {
    var contacts = await _mediator.Send(new GetContactsQuery());
    return Ok(contacts);
  }

  [HttpGet("{id:guid}")]
  public async Task<ActionResult<Contact>> GetById(Guid id)
  {
    var query = new GetContactByIdQuery { Id = id };
    var contact = await _mediator.Send(query);
    return Ok(contact);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactCommand command)
  {
    if (id != command.Id)
    {
      return BadRequest("ID mismatch between route and request body.");
    }
    await _mediator.Send(command);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var command = new DeleteContactCommand(id);
    await _mediator.Send(command);
    return NoContent();
  }

  [HttpGet("{contactId:guid}/events")]
  public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsForContact(Guid contactId)
  {
    var query = new GetEventsForContactQuery { ContactId = contactId };
    var events = await _mediator.Send(query);
    return Ok(events);
  }

  // --- NEW: Email Logs Endpoint ---
  [HttpGet("{contactId:guid}/email-logs")]
  public async Task<ActionResult<IEnumerable<EmailLogDto>>> GetEmailLogs(Guid contactId)
  {
    // Note: Ensure GetEmailLogsForContactQuery accepts the Guid in constructor or property init
    var query = new GetEmailLogsForContactQuery(contactId);
    var logs = await _mediator.Send(query);
    return Ok(logs);
  }
}