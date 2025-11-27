// src/Application/Vendors/Queries/GetEventsForVendorQueryHandler.cs
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Vendors.Queries
{
  public class GetEventsForVendorQueryHandler : IRequestHandler<GetEventsForVendorQuery, List<EventDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEventsForVendorQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<List<EventDto>> Handle(GetEventsForVendorQuery request, CancellationToken cancellationToken)
    {

      var events = await _context.EventVendors
          .Where(ev => ev.VendorId == request.VendorId)
          .Select(ev => ev.Event)
          // FIX: Use the correct property name 'StartDateTime'
          .OrderBy(e => e!.StartDateTime)
          .ToListAsync(cancellationToken);

      return _mapper.Map<List<EventDto>>(events);
    }
  }
}