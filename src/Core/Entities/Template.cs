using CRM_Vivid.Core.Enum;

namespace CRM_Vivid.Core.Entities;

public class Template
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Subject { get; set; }
  public string Content { get; set; } = string.Empty;
  public TemplateType Type { get; set; }
  public bool IsDeleted { get; set; } = false;
}