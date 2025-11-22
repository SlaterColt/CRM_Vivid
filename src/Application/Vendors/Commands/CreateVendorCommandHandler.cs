using AutoMapper;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;

namespace CRM_Vivid.Application.Vendors.Commands;

public class CreateVendorCommandHandler : IRequestHandler<CreateVendorCommand, Guid>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public CreateVendorCommandHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<Guid> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
  {
    var vendor = _mapper.Map<Vendor>(request);

    _context.Vendors.Add(vendor);
    await _context.SaveChangesAsync(cancellationToken);

    return vendor.Id;
  }
}