// src/Application/Events/Commands/AddVendorToEventCommandHandler.cs
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using CRM_Vivid.Application.Exceptions;

namespace CRM_Vivid.Application.Events.Commands
{
  public class AddVendorToEventCommandHandler : IRequestHandler<AddVendorToEventCommand, Guid>
  {
    private readonly IApplicationDbContext _context;

    public AddVendorToEventCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Guid> Handle(AddVendorToEventCommand request, CancellationToken cancellationToken)
    {
      if (await _context.Events.FindAsync(request.EventId) == null)
      {
        throw new NotFoundException(nameof(Event), request.EventId);
      }

      if (await _context.Vendors.FindAsync(request.VendorId) == null)
      {
        throw new NotFoundException(nameof(Vendor), request.VendorId);
      }

      var eventVendor = new EventVendor
      {
        EventId = request.EventId,
        VendorId = request.VendorId
      };

      await _context.EventVendors.AddAsync(eventVendor, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);

      return eventVendor.Id;
    }
  }
}