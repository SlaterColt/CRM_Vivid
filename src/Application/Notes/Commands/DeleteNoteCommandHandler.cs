using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;

namespace CRM_Vivid.Application.Notes.Commands;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand>
{
  private readonly IApplicationDbContext _context;

  public DeleteNoteCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  // --- FIX IS HERE ---
  // Fully qualify the 'Task' return type to avoid ambiguity
  public async System.Threading.Tasks.Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
  {
    var note = await _context.Notes.FindAsync(new object[] { request.Id }, cancellationToken);

    if (note == null)
    {
      throw new NotFoundException(nameof(Note), request.Id);
    }

    _context.Notes.Remove(note);
    await _context.SaveChangesAsync(cancellationToken);
  }
}