namespace CRM_Vivid.Application.Common.Models
{
  public class TaskDto
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty; // Enums will be mapped to strings
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? EventId { get; set; }
    public Guid? VendorId { get; set; }
    public string? VendorName { get; set; }

  }
}