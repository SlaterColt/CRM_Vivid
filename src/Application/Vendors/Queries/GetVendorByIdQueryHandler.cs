using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Vendors.Queries;

public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, VendorDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetVendorByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<VendorDto> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
  {
    var vendorDto = await _context.Vendors
        .Where(v => v.Id == request.Id)
        .ProjectTo<VendorDto>(_mapper.ConfigurationProvider)
        .SingleOrDefaultAsync(cancellationToken);

    if (vendorDto == null)
    {
      throw new NotFoundException(nameof(Vendor), request.Id);
    }

    return vendorDto;
  }
}