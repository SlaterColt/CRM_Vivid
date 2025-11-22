using CRM_Vivid.Application.Exceptions; // FIX: Correct namespace
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Core.Enum;
using MediatR;
using Task = System.Threading.Tasks.Task; // FIX: Resolve ambiguity with Entity Task

namespace CRM_Vivid.Application.Templates.Commands;

public class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand>
{
  private readonly IApplicationDbContext _context;

  public UpdateTemplateCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
  {
    var entity = await _context.Templates
        .FindAsync(new object[] { request.Id }, cancellationToken);

    if (entity == null)
    {
      throw new NotFoundException(nameof(Template), request.Id);
    }

    entity.Name = request.Name;
    entity.Subject = request.Subject;
    entity.Content = request.Content;

    // Ensure Enum parsing handles the case-insensitive string
    if (Enum.TryParse<TemplateType>(request.Type, true, out var typeEnum))
    {
      entity.Type = typeEnum;
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}