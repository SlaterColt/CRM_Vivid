using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Vendors.Queries;

public class GetVendorsQueryHandler : IRequestHandler<GetVendorsQuery, IEnumerable<VendorDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetVendorsQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<IEnumerable<VendorDto>> Handle(GetVendorsQuery request, CancellationToken cancellationToken)
  {
    return await _context.Vendors
        .ProjectTo<VendorDto>(_mapper.ConfigurationProvider)
        .OrderBy(v => v.Name)
        .ToListAsync(cancellationToken);
  }
}