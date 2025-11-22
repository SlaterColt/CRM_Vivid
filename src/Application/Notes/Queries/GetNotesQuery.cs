using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Notes.Queries;

// This query allows fetching all notes, or filtering by a specific parent entity
public class GetNotesQuery : IRequest<IEnumerable<NoteDto>>
{
  public Guid? ContactId { get; set; }
  public Guid? EventId { get; set; }
  public Guid? TaskId { get; set; }
  public Guid? VendorId { get; set; }
}