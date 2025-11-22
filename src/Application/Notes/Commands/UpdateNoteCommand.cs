using MediatR;

namespace CRM_Vivid.Application.Notes.Commands;

public class UpdateNoteCommand : IRequest
{
  public Guid Id { get; set; }
  public string Content { get; set; } = string.Empty;

  // Note: We are only allowing Content to be updated.
  // Re-associating a note is a complex business rule
  // (e.g., delete and recreate).
}