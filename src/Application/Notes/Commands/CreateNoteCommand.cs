// FILE: src/Application/Notes/Commands/CreateNoteCommand.cs
using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Notes.Commands;

public class CreateNoteCommand : IRequest<NoteDto>
{
  public string Content { get; set; } = string.Empty;

  // --- Original GUID Foreign Keys ---
  public Guid? ContactId
  {
    get;
    set;
  }
  public Guid? EventId { get; set; }
  public Guid? TaskId
  {
    get; set;
  }
  public Guid? VendorId { get; set; }

  // --- NEW: PHASE 29 STRING-BASED RESOLUTION FIELDS ---
  public string? ContactEmail { get; set; }
  public string? VendorName { get; set; }
  public string? EventName { get; set; }
  public string? TaskTitle { get; set; } // Notes can attach to tasks, so we need task resolution
}