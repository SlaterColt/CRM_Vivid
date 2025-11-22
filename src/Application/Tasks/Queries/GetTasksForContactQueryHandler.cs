using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Tasks.Queries
{
  public class GetTasksForContactQueryHandler : IRequestHandler<GetTasksForContactQuery, IEnumerable<TaskDto>>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTasksForContactQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<IEnumerable<TaskDto>> Handle(GetTasksForContactQuery request, CancellationToken cancellationToken)
    {
      return await _context.Tasks
          .AsNoTracking()
          .Where(t => t.ContactId == request.ContactId)
          .OrderBy(t => t.DueDate)
          .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
          .ToListAsync(cancellationToken);
    }
  }
}