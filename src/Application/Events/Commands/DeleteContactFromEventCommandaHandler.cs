// src/Application/Events/Commands/DeleteContactFromEventCommandHandler.cs

using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Events.Commands;

public record DeleteContactFromEventCommand : IRequest<Unit>
{
  public Guid EventId { get; init; }
  public Guid ContactId { get; init; }
}

public class DeleteContactFromEventCommandHandler : IRequestHandler<DeleteContactFromEventCommand, Unit>
{
  private readonly IApplicationDbContext _context;

  public DeleteContactFromEventCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<Unit> Handle(DeleteContactFromEventCommand request, CancellationToken cancellationToken)
  {
    // Find the specific join entity (EventContact)
    var entity = await _context.EventContacts
        .FirstOrDefaultAsync(ec => ec.EventId == request.EventId && ec.ContactId == request.ContactId, cancellationToken);

    if (entity == null)
    {
      throw new NotFoundException($"Contact with ID {request.ContactId} is not linked to Event with ID {request.EventId}.");
    }

    _context.EventContacts.Remove(entity);

    await _context.SaveChangesAsync(cancellationToken);

    return Unit.Value; // Protocol 2 Compliance: Returns Unit for a Command with no return data
  }
}