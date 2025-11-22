using MediatR;

namespace CRM_Vivid.Application.Templates.Commands;

public record UpdateTemplateCommand : IRequest
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Subject { get; set; }
  public string Content { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
}