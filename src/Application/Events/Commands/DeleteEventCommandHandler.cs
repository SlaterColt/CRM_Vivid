// src/Application/Events/Commands/DeleteEventCommandHandler.cs
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Events.Commands;

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand>
{
  private readonly IApplicationDbContext _context;

  public DeleteEventCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task Handle(DeleteEventCommand request, CancellationToken cancellationToken)
  {
    var eventItem = await _context.Events
        .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

    if (eventItem == null)
    {
      // We can replace this with a custom NotFoundException later
      throw new Exception($"Event with ID {request.Id} not found.");
    }

    _context.Events.Remove(eventItem);

    await _context.SaveChangesAsync(cancellationToken);
  }
}