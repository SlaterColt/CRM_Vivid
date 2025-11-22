using AutoMapper;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;

namespace CRM_Vivid.Application.Tasks.Commands
{
  public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateTaskCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
      var entity = await _context.Tasks.FindAsync(new object[] { request.Id }, cancellationToken);

      if (entity == null)
      {
        throw new NotFoundException(nameof(Core.Entities.Task), request.Id);
      }

      _mapper.Map(request, entity);

      // Per Rule #2: Use UTC
      if (request.DueDate.HasValue)
      {
        entity.DueDate = request.DueDate.Value.ToUniversalTime();
      }
      entity.UpdatedAt = DateTime.UtcNow;

      _context.Tasks.Update(entity);
      await _context.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}