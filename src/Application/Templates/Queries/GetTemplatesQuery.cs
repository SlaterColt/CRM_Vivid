using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Templates.Queries;

public record GetTemplatesQuery : IRequest<List<TemplateDto>>;

public class GetTemplatesQueryHandler : IRequestHandler<GetTemplatesQuery, List<TemplateDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetTemplatesQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<List<TemplateDto>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
  {
    return await _context.Templates
        .AsNoTracking()
        .ProjectTo<TemplateDto>(_mapper.ConfigurationProvider)
        .OrderBy(t => t.Name)
        .ToListAsync(cancellationToken);
  }
}