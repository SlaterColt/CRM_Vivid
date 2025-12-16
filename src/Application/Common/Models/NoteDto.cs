namespace CRM_Vivid.Application.Common.Models;

public class NoteDto
{
  public Guid Id { get; set; }
  public string Content { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  // Simple FKs for the DTO
  public Guid? ContactId { get; set; }
  public Guid? EventId { get; set; }
  public Guid? TaskId { get; set; }
  public Guid? VendorId { get; set; }
  public string? VendorName { get; set; }
}