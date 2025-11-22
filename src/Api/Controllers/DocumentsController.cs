using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Documents.Commands;
using CRM_Vivid.Application.Documents.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Vivid.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
  private readonly IMediator _mediator;

  public DocumentsController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<DocumentDto>> Upload(
      [FromForm] IFormFile file,
      [FromForm] Guid relatedEntityId,
      [FromForm] string relatedEntityType,
      [FromForm] string category = "General") // NEW: Accept Category
  {
    if (file == null || file.Length == 0)
    {
      return BadRequest("No file uploaded.");
    }

    using var stream = file.OpenReadStream();

    var command = new UploadDocumentCommand
    {
      FileName = file.FileName,
      ContentType = file.ContentType,
      Size = file.Length,
      FileContent = stream,
      RelatedEntityId = relatedEntityId,
      RelatedEntityType = relatedEntityType,
      Category = category // Pass to Command
    };

    var result = await _mediator.Send(command);

    return Ok(result);
  }

  [HttpGet]
  public async Task<ActionResult<List<DocumentDto>>> Get(
      [FromQuery] Guid relatedEntityId,
      [FromQuery] string relatedEntityType)
  {
    var query = new GetDocumentsQuery
    {
      RelatedEntityId = relatedEntityId,
      RelatedEntityType = relatedEntityType
    };

    var result = await _mediator.Send(query);
    return Ok(result);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _mediator.Send(new DeleteDocumentCommand(id));
    return NoContent();
  }
}