using AutoMapper;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;


namespace CRM_Vivid.Application.Tasks.Commands
{
  public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
      var entity = _mapper.Map<Core.Entities.Task>(request);

      // Per Rule #2: Use UTC
      if (request.DueDate.HasValue)
      {
        entity.DueDate = request.DueDate.Value.ToUniversalTime();
      }

      entity.CreatedAt = DateTime.UtcNow;

      await _context.Tasks.AddAsync(entity, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);

      return entity.Id;
    }
  }
}