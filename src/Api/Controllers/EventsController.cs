// FILE: src/Api/Controllers/EventsController.cs

using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Events.Commands;
using CRM_Vivid.Application.Events.Queries;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CRM_Vivid.Application.Exceptions;

namespace CRM_Vivid.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class EventsController : ControllerBase
  {
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateEventCommand command)
    {
      var id = await _mediator.Send(command);
      return CreatedAtAction(nameof(Create), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetAll()
    {
      var events = await _mediator.Send(new GetEventsQuery());
      return Ok(events);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventDto>> GetEventById(Guid id)
    {
      var query = new GetEventByIdQuery { Id = id };
      var eventItem = await _mediator.Send(query);

      if (eventItem == null)
      {
        return NotFound();
      }

      return Ok(eventItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(Guid id, UpdateEventCommand command)
    {
      if (id != command.Id)
      {
        return BadRequest("ID mismatch between URL and request body.");
      }

      try
      {
        await _mediator.Send(command);
      }
      catch (Exception ex)
      {
        return NotFound(ex.Message);
      }

      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
      var command = new DeleteEventCommand { Id = id };

      try
      {
        await _mediator.Send(command);
      }
      catch (Exception ex)
      {
        return NotFound(ex.Message);
      }

      return NoContent();
    }

    [HttpPost("{eventId}/contacts")]
    public async Task<IActionResult> AddContactToEvent(Guid eventId, [FromBody] AddContactToEventCommand command)
    {
      command.EventId = eventId;

      try
      {
        await _mediator.Send(command);
      }
      // Note: Handler now throws NotFoundException and ArgumentException for links
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
      return Ok();
    }

    [HttpGet("{eventId:guid}/contacts")]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetContactsForEvent(Guid eventId)
    {
      var query = new GetContactsForEventQuery { EventId = eventId };
      // Handler now uses explicit projection to include the Role
      var contacts = await _mediator.Send(query);

      return Ok(contacts);
    }

    // NEW: Delete Contact from Event (Unlink)
    [HttpDelete("{eventId:guid}/contacts/{contactId:guid}")]
    public async Task<IActionResult> DeleteContactFromEvent(Guid eventId, Guid contactId)
    {
      var command = new DeleteContactFromEventCommand
      {
        EventId = eventId,
        ContactId = contactId
      };

      try
      {
        await _mediator.Send(command);
      }
      // Uses the now-imported NotFoundException type
      catch (NotFoundException ex)
      {
        return NotFound(ex.Message);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

      return NoContent(); // 204 success, no content returned
    }

    // --- PHASE 14 VENDOR ENDPOINTS START HERE ---

    // POST api/events/{id}/vendors (Now accepts Role)
    [HttpPost("{id}/vendors")]
    public async Task<ActionResult<Guid>> AddVendorToEvent(Guid id, [FromBody] AddVendorToEventCommand command)
    {
      if (id != command.EventId)
      {
        return BadRequest("Event ID in path must match Event ID in body.");
      }
      // Handler now assigns Role and is hardened
      var eventVendorId = await _mediator.Send(command);
      return Ok(eventVendorId);
    }

    // --- PHASE 34 ADDITION: Get Vendors for Event with Roles ---
    [HttpGet("{eventId:guid}/vendors")]
    public async Task<ActionResult<IEnumerable<VendorDto>>> GetVendorsForEvent(Guid eventId)
    {
      // Handler uses explicit projection to include the Role
      var query = new GetVendorsForEventQuery { EventId = eventId };
      var vendors = await _mediator.Send(query);
      return Ok(vendors);
    }

    // DELETE api/events/{id}/vendors/{vendorId}
    [HttpDelete("{id}/vendors/{vendorId}")]
    public async Task<ActionResult> DeleteVendorFromEvent(Guid id, Guid vendorId)
    {
      // FIX: Use the injected _mediator field
      await _mediator.Send(new DeleteVendorFromEventCommand { EventId = id, VendorId = vendorId });
      return NoContent();
    }
  } // FINAL closing brace for EventsController
}