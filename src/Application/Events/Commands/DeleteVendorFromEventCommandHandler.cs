// src/Application/Events/Commands/DeleteVendorFromEventCommandHandler.cs
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Events.Commands
{
  // FIX: Ensure IRequestHandler is returning the correct Task type (Task<Unit>)
  public class DeleteVendorFromEventCommandHandler : IRequestHandler<DeleteVendorFromEventCommand>
  {
    private readonly IApplicationDbContext _context;

    public DeleteVendorFromEventCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    // The method signature must match exactly
    public async Task<Unit> Handle(DeleteVendorFromEventCommand request, CancellationToken cancellationToken)
    {
      var eventVendor = await _context.EventVendors
          .FirstOrDefaultAsync(ev => ev.EventId == request.EventId && ev.VendorId == request.VendorId, cancellationToken);

      if (eventVendor == null)
      {
        return Unit.Value;
      }

      _context.EventVendors.Remove(eventVendor);
      await _context.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }

    System.Threading.Tasks.Task IRequestHandler<DeleteVendorFromEventCommand>.Handle(DeleteVendorFromEventCommand request, CancellationToken cancellationToken)
    {
      return Handle(request, cancellationToken);
    }
  }
}