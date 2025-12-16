namespace CRM_Vivid.Core.Entities;

public class Note : IUserScopedEntity
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty; // Added default
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  // Polymorphic Foreign Keys
  public Guid? ContactId { get; set; }
  public Contact? Contact { get; set; }

  public Guid? EventId { get; set; }
  public Event? Event { get; set; }

  public Guid? TaskId { get; set; }
  public Task? Task { get; set; }

  public Guid? VendorId { get; set; }
  public Vendor? Vendor { get; set; }
  public Guid CreatedByUserId { get; set; }
}