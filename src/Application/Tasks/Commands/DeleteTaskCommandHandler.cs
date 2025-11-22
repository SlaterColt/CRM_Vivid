using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Tasks.Commands
{
  // This should be the ONLY class in this file
  public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
  {
    private readonly IApplicationDbContext _context;

    public DeleteTaskCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
      var entity = await _context.Tasks
          .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

      if (entity == null)
      {
        throw new NotFoundException(nameof(Core.Entities.Task), request.Id);
      }

      _context.Tasks.Remove(entity);
      await _context.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}