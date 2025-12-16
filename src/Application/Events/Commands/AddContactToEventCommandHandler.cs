// FILE: src/Application/Events/Commands/AddContactToEventCommandHandler.cs (MODIFIED)
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM_Vivid.Application.Exceptions; // NEW: Import the custom exception

namespace CRM_Vivid.Application.Events.Commands;

public class AddContactToEventCommandHandler : IRequestHandler<AddContactToEventCommand>
{
  private readonly IApplicationDbContext _context;
  private readonly ICurrentUserService _currentUserService;

  public AddContactToEventCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
  {
    _context = context;
    _currentUserService = currentUserService;
  }

  public async System.Threading.Tasks.Task Handle(AddContactToEventCommand request, CancellationToken cancellationToken)
  {
    // 1. Check if the Event exists
    var eventExists = await _context.Events
        .AnyAsync(e => e.Id == request.EventId, cancellationToken);
    if (!eventExists)
    {
      // FIX 1: Use NotFoundException
      throw new NotFoundException(nameof(Event), request.EventId);
    }

    // 2. Check if the Contact exists
    var contactExists = await _context.Contacts
        .AnyAsync(c => c.Id == request.ContactId, cancellationToken);
    if (!contactExists)
    {
      // FIX 2: Use NotFoundException
      throw new NotFoundException(nameof(Contact), request.ContactId);
    }

    // Optional: Check if the link already exists
    var linkExists = await _context.EventContacts
        .AnyAsync(ec => ec.EventId == request.EventId && ec.ContactId == request.ContactId, cancellationToken);
    if (linkExists)
    {
      // FIX 3: Use ArgumentException for business logic violation
      throw new ArgumentException($"Contact with ID {request.ContactId} is already linked to event {request.EventId}.");
    }

    // 3. Create the new EventContact entity
    var eventContact = new EventContact
    {
      EventId = request.EventId,
      ContactId = request.ContactId,
      // Role mapping is correct for Phase 34
      Role = request.Role ?? string.Empty,
      CreatedByUserId = _currentUserService.CurrentUserId
    };

    // 4. Add it to the DbSet
    _context.EventContacts.Add(eventContact);

    // 5. Save changes
    await _context.SaveChangesAsync(cancellationToken);
  }
}