using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Documents.Commands;
using CRM_Vivid.Application.Documents.Queries;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Enum; // NEW: Needed for ContractStatus in Webhook action
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Vivid.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly IConfiguration _configuration; // NEW: Configuration to check Webhook Secret

  // MODIFIED CONSTRUCTOR
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

  // --- NEW ENDPOINT: PHASE 24 (CONTRACT GENERATOR) ---
  [HttpGet("contract/{eventId}")]
  [Produces("application/pdf")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult> GenerateContract(Guid eventId)
  {
    var command = new GenerateContractCommand { EventId = eventId };

    try
    {
      // Note: The Command Handler is responsible for calling IContractGenerator
      var pdfBytes = await _mediator.Send(command);

      var fileName = $"Contract_{eventId}_{DateTime.Now:yyyyMMdd}.pdf";

      // Return the byte array as a file download
      return File(
          fileContents: pdfBytes,
          contentType: "application/pdf",
          fileDownloadName: fileName
      );
    }
    catch (NotFoundException)
    {
      // Catch the specific exception thrown by the command handler if the event doesn't exist
      return NotFound(new { message = $"Event ID {eventId} not found." });
    }
  }

  // --- PHASE 26 ADDITION: THE WEBHOOK LISTENER ---
  [HttpPost("webhook")]
  [Microsoft.AspNetCore.Authorization.AllowAnonymous]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public async Task<IActionResult> Webhook([FromBody] SimulatedWebhookRequest request)
  {
    // 1. Security: Check the Webhook Secret (Keep it simple but secure for now)
    var expectedSecret = _configuration["Secrets:ContractWebhookSecret"];

    if (string.IsNullOrEmpty(expectedSecret) || request.WebhookSecret != expectedSecret)
    {
      // Log failure attempt here
      return Unauthorized("Invalid webhook secret.");
    }

    // 2. Mapping: Map the incoming Status string to our internal ContractStatus enum
    ContractStatus newStatus;

    if (!Enum.TryParse(request.Status, true, out newStatus))
    {
      return BadRequest($"Invalid status value received: {request.Status}");
    }

    // 3. Command: Trigger the internal state change logic
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
      // Catch other errors (e.g., database failure) and return a 500
      return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Webhook processing failed.", error = ex.Message });
    }
  }
}