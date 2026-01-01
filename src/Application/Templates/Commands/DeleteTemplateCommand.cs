using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using Task = System.Threading.Tasks.Task;

namespace CRM_Vivid.Application.Templates.Commands;

public record DeleteTemplateCommand(Guid Id) : IRequest;

public class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand>
{
  private readonly IApplicationDbContext _context;

  public DeleteTemplateCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
  {
    var entity = await _context.Templates
        .FindAsync(new object[] { request.Id }, cancellationToken);

    if (entity == null)
    {
      throw new NotFoundException(nameof(Template), request.Id);
    }

    entity.IsDeleted = true;

    await _context.SaveChangesAsync(cancellationToken);
  }
}