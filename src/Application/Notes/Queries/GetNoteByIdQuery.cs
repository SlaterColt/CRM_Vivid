using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Notes.Queries;

public class GetNoteByIdQuery : IRequest<NoteDto>
{
  public Guid Id { get; set; }
}