// FILE: src/Application/Events/Commands/AddVendorToEventCommandHandler.cs (MODIFIED)
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using CRM_Vivid.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

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
      // 1. Check if Event exists
      if (await _context.Events.FindAsync(new object[] { request.EventId }, cancellationToken) == null)
      {
        throw new NotFoundException(nameof(Event), request.EventId);
      }

      // 2. Check if Vendor exists
      if (await _context.Vendors.FindAsync(new object[] { request.VendorId }, cancellationToken) == null)
      {
        throw new NotFoundException(nameof(Vendor), request.VendorId);
      }

      // 3. Check for existing link (Unique constraint enforcement)
      var linkExists = await _context.EventVendors
          .AnyAsync(ev => ev.EventId == request.EventId && ev.VendorId == request.VendorId, cancellationToken);

      if (linkExists)
      {
        throw new ArgumentException($"Vendor with ID {request.VendorId} is already linked to event {request.EventId}.");
      }

      // 4. Create join entity and assign role
      var eventVendor = new EventVendor
      {
        EventId = request.EventId,
        VendorId = request.VendorId,
        // --- NEW: PHASE 34 ASSIGN THE ROLE ---
        Role = request.Role
      };

      await _context.EventVendors.AddAsync(eventVendor, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);

      return eventVendor.Id;
    }
  }
}