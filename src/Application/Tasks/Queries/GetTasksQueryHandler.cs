// src/Application/Tasks/Queries/GetTasksQueryHandler.cs
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Tasks.Queries
{
  public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IEnumerable<TaskDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTasksQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<IEnumerable<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
      // Start with a base queryable
      var query = _context.Tasks
          .AsNoTracking()
          .AsQueryable();

      // Conditionally apply filters
      if (request.ContactId.HasValue)
      {
        query = query.Where(t => t.ContactId == request.ContactId);
      }

      if (request.EventId.HasValue)
      {
        query = query.Where(t => t.EventId == request.EventId);
      }

      // NEW: Apply Vendor Filter
      if (request.VendorId.HasValue)
      {
        query = query.Where(t => t.VendorId == request.VendorId);
      }

      // Project and execute
      return await query
          .OrderByDescending(t => t.CreatedAt)
          .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
          .ToListAsync(cancellationToken);
    }
  }
}