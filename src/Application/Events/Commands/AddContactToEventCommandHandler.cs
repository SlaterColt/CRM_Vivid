// src/Application/Events/Commands/AddContactToEventCommandHandler.cs
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Events.Commands;

public class AddContactToEventCommandHandler : IRequestHandler<AddContactToEventCommand>
{
  private readonly IApplicationDbContext _context;

  public AddContactToEventCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async System.Threading.Tasks.Task Handle(AddContactToEventCommand request, CancellationToken cancellationToken)
  {
    // 1. Check if the Event exists
    var eventExists = await _context.Events
        .AnyAsync(e => e.Id == request.EventId, cancellationToken);
    if (!eventExists)
    {
      throw new Exception($"Event with ID {request.EventId} not found.");
    }

    // 2. Check if the Contact exists
    var contactExists = await _context.Contacts
        .AnyAsync(c => c.Id == request.ContactId, cancellationToken);
    if (!contactExists)
    {
      throw new Exception($"Contact with ID {request.ContactId} not found.");
    }

    // Optional: Check if the link already exists
    var linkExists = await _context.EventContacts
        .AnyAsync(ec => ec.EventId == request.EventId && ec.ContactId == request.ContactId, cancellationToken);
    if (linkExists)
    {
      // Or you could just return silently
      throw new Exception("This contact is already linked to this event.");
    }

    // 3. Create the new EventContact entity
    var eventContact = new EventContact
    {
      EventId = request.EventId,
      ContactId = request.ContactId,
      Role = request.Role ?? string.Empty
    };

    // 4. Add it to the DbSet
    _context.EventContacts.Add(eventContact);

    // 5. Save changes
    await _context.SaveChangesAsync(cancellationToken);

  }
}