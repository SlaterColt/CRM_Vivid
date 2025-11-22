// src/Application/Events/Commands/UpdateEventCommandHandler.cs
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Enums; // <-- Import the enum
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Events.Commands;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand>
{
  private readonly IApplicationDbContext _context;

  public UpdateEventCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
  {
    var eventItem = await _context.Events
        .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

    if (eventItem == null)
    {
      // Later, we can replace this with a custom NotFoundException
      throw new Exception($"Event with ID {request.Id} not found.");
    }

    // --- Parse the status string ---
    if (!Enum.TryParse<EventStatus>(request.Status, true, out var eventStatus))
    {
      throw new ArgumentException($"Invalid event status: {request.Status}. Valid values are Planned, InProgress, Completed, Postponed, Cancelled.");
    }
    // -------------------------------

    // Update properties based on your Event.cs file
    eventItem.Name = request.Name;
    eventItem.Status = eventStatus; // Use the parsed enum
    eventItem.StartDateTime = request.StartDateTime.ToUniversalTime();
    eventItem.EndDateTime = request.EndDateTime.ToUniversalTime();
    eventItem.IsPublic = request.IsPublic;
    eventItem.Description = request.Description;
    eventItem.Location = request.Location;
    // We don't have an 'UpdatedAt' field on your entity, so we'll skip it.


    await _context.SaveChangesAsync(cancellationToken);
  }
}