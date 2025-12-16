// FILE: src/Application/Common/Models/ActivityDto.cs (NEW FILE)
namespace CRM_Vivid.Application.Common.Models;

/// <summary>
/// A unified DTO for displaying tasks, notes, and email logs in a single timeline.
/// </summary>
public class ActivityDto
{
  public Guid Id { get; set; }

  // The timestamp used for chronological ordering
  public DateTime Timestamp { get; set; }

  // The type of activity (e.g., "TASK", "NOTE", "EMAIL")
  public string ActivityType { get; set; } = string.Empty;

  // A short summary or title
  public string Title { get; set; } = string.Empty;

  // The main content/description (optional)
  public string? Content { get; set; }

  // Status/Metadata specific to the type (e.g., "Completed", "Sent", "Vendor Inquiry")
  public string? Status { get; set; }

  // Optional foreign key to a related entity, if applicable
  public Guid? RelatedEntityId { get; set; }
}