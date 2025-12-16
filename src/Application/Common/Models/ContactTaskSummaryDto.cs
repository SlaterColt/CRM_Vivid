// FILE: src/Application/Common/Models/ContactTaskSummaryDto.cs (NEW FILE)
namespace CRM_Vivid.Application.Common.Models;

public class ContactTaskSummaryDto
{
  public Guid ContactId { get; set; }
  public string ContactName { get; set; } = string.Empty;

  // Phase 35 Reporting Metrics
  public int TotalTasksAssigned { get; set; }
  public int TotalTasksCompleted { get; set; }
  public int TotalTasksOverdue { get; set; }
}