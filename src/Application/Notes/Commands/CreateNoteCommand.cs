using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Notes.Commands;

public class CreateNoteCommand : IRequest<NoteDto>
{
  public string Content { get; set; } = string.Empty;
  public Guid? ContactId { get; set; }
  public Guid? EventId { get; set; }
  public Guid? TaskId { get; set; }
  public Guid? VendorId { get; set; }
}