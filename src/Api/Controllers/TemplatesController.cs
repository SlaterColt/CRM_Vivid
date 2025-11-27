using CRM_Vivid.Application.Templates.Commands;
using CRM_Vivid.Application.Templates.Queries;
using CRM_Vivid.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace CRM_Vivid.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TemplatesController : ControllerBase
{
  private readonly ISender _sender;

  public TemplatesController(ISender sender)
  {
    _sender = sender;
  }

  [HttpGet]
  public async Task<ActionResult<List<TemplateDto>>> GetTemplates()
  {
    return await _sender.Send(new GetTemplatesQuery());
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TemplateDto>> GetTemplate(Guid id)
  {
    return await _sender.Send(new GetTemplateByIdQuery(id));
  }

  [HttpPost]
  public async Task<ActionResult<Guid>> Create(CreateTemplateCommand command)
  {
    return await _sender.Send(command);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult> Update(Guid id, UpdateTemplateCommand command)
  {
    if (id != command.Id)
    {
      return BadRequest();
    }

    await _sender.Send(command);

    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(Guid id)
  {
    await _sender.Send(new DeleteTemplateCommand(id));
    return NoContent();
  }

  // --- NEW ENDPOINT ---
  [HttpPost("send")]
  public async Task<ActionResult> SendTemplate(SendTemplateEmailCommand command)
  {
    await _sender.Send(command);
    return Ok();
  }
}