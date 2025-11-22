using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Tasks.Queries
{
  public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTaskByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
      var entity = await _context.Tasks
          .AsNoTracking()
          .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

      if (entity == null)
      {
        throw new NotFoundException(nameof(Core.Entities.Task), request.Id);
      }

      return _mapper.Map<TaskDto>(entity);
    }
  }
}