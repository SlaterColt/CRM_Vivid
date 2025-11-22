using CRM_Vivid.Application.Contacts.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CRM_Vivid.Api.Controllers;

[AllowAnonymous] // IMPORTANT: This allows external app/forms to submit data
[Route("api/[controller]")]
[ApiController]
public class LeadsController : ControllerBase
{
  private readonly ISender _sender;

  public LeadsController(ISender sender)
  {
    _sender = sender;
  }

  [HttpPost("submit")]
  [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<Guid>> SubmitLead(SubmitLeadCommand command)
  {
    // Validation handled by MediatR pipeline (FluentValidation)
    var contactId = await _sender.Send(command);
    return Ok(contactId);
  }
}