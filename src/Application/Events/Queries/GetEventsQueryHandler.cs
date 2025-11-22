using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Events.Queries
{
  public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, List<EventDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEventsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<List<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
      // We'll add .AsNoTracking() for a read-only query, which is more efficient.
      return await _context.Events
          .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
          .ToListAsync(cancellationToken);
    }
  }
}