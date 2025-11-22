using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;

namespace CRM_Vivid.Application.Notes.Commands;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand>
{
  private readonly IApplicationDbContext _context;

  public UpdateNoteCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  // --- FIX IS HERE ---
  // Fully qualify the 'Task' return type to avoid ambiguity
  public async System.Threading.Tasks.Task Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
  {
    var note = await _context.Notes.FindAsync(new object[] { request.Id }, cancellationToken);

    if (note == null)
    {
      throw new NotFoundException(nameof(Note), request.Id);
    }

    note.Content = request.Content;
    note.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync(cancellationToken);
  }
}