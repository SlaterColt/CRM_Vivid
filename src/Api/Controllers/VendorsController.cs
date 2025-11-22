// src/Api/Controllers/VendorsController.cs

using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Vendors.Commands;
using CRM_Vivid.Application.Vendors.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Vivid.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
  private readonly IMediator _mediator;

  public VendorsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateVendor([FromBody] CreateVendorCommand command)
  {
    var vendorId = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetVendorById), new { id = vendorId }, command);
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<VendorDto>>> GetVendors()
  {
    var vendors = await _mediator.Send(new GetVendorsQuery());
    return Ok(vendors);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<VendorDto>> GetVendorById(Guid id)
  {
    var vendor = await _mediator.Send(new GetVendorByIdQuery { Id = id });
    return Ok(vendor);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateVendor(Guid id, [FromBody] UpdateVendorCommand command)
  {
    if (id != command.Id)
    {
      return BadRequest("ID mismatch");
    }

    await _mediator.Send(command);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteVendor(Guid id)
  {
    await _mediator.Send(new DeleteVendorCommand { Id = id });
    return NoContent();
  }

  // NEW: Get Events For Vendor Endpoint
  [HttpGet("{id}/events")]
  public async Task<ActionResult<List<EventDto>>> GetEventsForVendor(Guid id)
  {
    // FIX: Use the injected private field _mediator
    var events = await _mediator.Send(new GetEventsForVendorQuery { VendorId = id });
    return Ok(events);
  }
}