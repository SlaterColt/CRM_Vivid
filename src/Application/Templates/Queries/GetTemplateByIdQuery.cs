using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Task = System.Threading.Tasks.Task; // Alias for System Task

namespace CRM_Vivid.Application.Templates.Queries;

public record GetTemplateByIdQuery(Guid Id) : IRequest<TemplateDto>;

public class GetTemplateByIdQueryHandler : IRequestHandler<GetTemplateByIdQuery, TemplateDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetTemplateByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<TemplateDto> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
  {
    var entity = await _context.Templates
        .FindAsync(new object[] { request.Id }, cancellationToken);

    if (entity == null)
    {
      throw new NotFoundException(nameof(Core.Entities.Template), request.Id);
    }

    return _mapper.Map<TemplateDto>(entity);
  }
}