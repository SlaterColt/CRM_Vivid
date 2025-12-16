using AutoMapper;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;

namespace CRM_Vivid.Application.Vendors.Commands;

public class CreateVendorCommandHandler : IRequestHandler<CreateVendorCommand, Guid>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;
  private readonly ICurrentUserService _currentUserService;

  public CreateVendorCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
  {
    _context = context;
    _mapper = mapper;
    _currentUserService = currentUserService;
  }

  public async Task<Guid> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
  {
    var vendor = _mapper.Map<Vendor>(request);

    // --- PHASE 37: ASSIGN OWNER ---
    vendor.CreatedByUserId = _currentUserService.CurrentUserId;

    _context.Vendors.Add(vendor);
    await _context.SaveChangesAsync(cancellationToken);

    return vendor.Id;
  }
}