using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using CRM_Vivid.Core.Enums;

namespace CRM_Vivid.Application.Events.Commands
{
  public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
  {
    private readonly IApplicationDbContext _context;

    public CreateEventCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {

      var newEvent = new Event
      {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Status = EventStatus.Planned,
        StartDateTime = request.StartDateTime.ToUniversalTime(),
        EndDateTime = request.EndDateTime.ToUniversalTime(),
        IsPublic = request.IsPublic,
        Description = request.Description,
        Location = request.Location
      };

      _context.Events.Add(newEvent);

      await _context.SaveChangesAsync(cancellationToken);

      return newEvent.Id;
    }
  }
}