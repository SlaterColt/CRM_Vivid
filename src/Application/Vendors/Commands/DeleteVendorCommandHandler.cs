using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Vendors.Commands;

public class DeleteVendorCommandHandler : IRequestHandler<DeleteVendorCommand, bool>
{
  private readonly IApplicationDbContext _context;

  public DeleteVendorCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<bool> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
  {
    var vendor = await _context.Vendors
        .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

    if (vendor == null)
    {
      throw new NotFoundException(nameof(Vendor), request.Id);
    }

    _context.Vendors.Remove(vendor);
    await _context.SaveChangesAsync(cancellationToken);

    return true;
  }
}