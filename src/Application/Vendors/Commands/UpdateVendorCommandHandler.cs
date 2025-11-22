using AutoMapper;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Vendors.Commands;

public class UpdateVendorCommandHandler : IRequestHandler<UpdateVendorCommand, bool>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public UpdateVendorCommandHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<bool> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
  {
    var vendor = await _context.Vendors
        .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

    if (vendor == null)
    {
      throw new NotFoundException(nameof(Vendor), request.Id);
    }

    _mapper.Map(request, vendor);

    await _context.SaveChangesAsync(cancellationToken);

    return true;
  }
}