using MediatR;

namespace CRM_Vivid.Application.Notes.Commands;

public class DeleteNoteCommand : IRequest
{
  public Guid Id { get; set; }
}