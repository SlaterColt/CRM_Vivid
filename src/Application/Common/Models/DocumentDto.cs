namespace CRM_Vivid.Application.Common.Models;

public class DocumentDto
{
  public int Id { get; set; }
  public required string FileName { get; set; }
  public required string ContentType { get; set; }
  public long Size { get; set; }
  public DateTime UploadedAt { get; set; }
  public required string Url { get; set; }
  public Guid RelatedEntityId { get; set; }
  public required string RelatedEntityType { get; set; }

  // --- NEW ---
  public required string Category { get; set; }
}