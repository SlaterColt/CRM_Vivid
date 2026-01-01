using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Documents.Commands;
using CRM_Vivid.Application.Documents.Queries;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace CRM_Vivid.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly IConfiguration _configuration;

  public DocumentsController(IMediator mediator, IConfiguration configuration)
  {
    _mediator = mediator;
    _configuration = configuration;
  }

  [HttpPost]
  public async Task<ActionResult<DocumentDto>> Upload(
      [FromForm] IFormFile file,
      [FromForm] Guid relatedEntityId,
      [FromForm] string relatedEntityType,
      [FromForm] string category = "General")
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
      Category = category
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

  // --- GENERATE CONTRACT ENDPOINT ---
  [HttpPost("generate-contract/{eventId}")]
  [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<DocumentDto>> GenerateContract(Guid eventId)
  {
    var command = new GenerateContractCommand { EventId = eventId };

    try
    {
      var result = await _mediator.Send(command);
      return Ok(result);
    }
    catch (NotFoundException)
    {
      return NotFound(new { message = $"Event ID {eventId} not found." });
    }
  }

  [HttpPost("webhook")]
  [Microsoft.AspNetCore.Authorization.AllowAnonymous]
  public async Task<IActionResult> Webhook([FromBody] SimulatedWebhookRequest request)
  {
    var expectedSecret = _configuration["Secrets:ContractWebhookSecret"];

    if (string.IsNullOrEmpty(expectedSecret) || request.WebhookSecret != expectedSecret)
    {
      return Unauthorized("Invalid webhook secret.");
    }

    ContractStatus newStatus;
    if (!Enum.TryParse(request.Status, true, out newStatus))
    {
      return BadRequest($"Invalid status value received: {request.Status}");
    }

    var command = new UpdateDocumentStatusCommand(
        DocumentId: request.DocumentId,
        NewStatus: newStatus
    );

    try
    {
      await _mediator.Send(command);
      return Ok(new { message = $"Document {request.DocumentId} status updated to {newStatus}." });
    }
    catch (NotFoundException ex)
    {
      return NotFound(new { message = ex.Message });
    }
    catch (Exception ex)
    {
      return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Webhook processing failed.", error = ex.Message });
    }
  }
}