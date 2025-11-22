using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Core.Enum;
using MediatR;

namespace CRM_Vivid.Application.Templates.Commands;

public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, Guid>
{
  private readonly IApplicationDbContext _context;

  public CreateTemplateCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<Guid> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
  {
    var entity = new Template
    {
      Id = Guid.NewGuid(),
      Name = request.Name,
      Subject = request.Subject,
      Content = request.Content,
      Type = Enum.Parse<TemplateType>(request.Type, true)
    };

    _context.Templates.Add(entity);
    await _context.SaveChangesAsync(cancellationToken);

    return entity.Id;
  }
}