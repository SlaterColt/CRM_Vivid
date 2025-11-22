namespace CRM_Vivid.Core.Entities;

public class Contact
{
  public Guid Id { get; set; } = Guid.NewGuid();

  public string FirstName { get; set; } = string.Empty;
  public string? LastName { get; set; }
  public string Email { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; }

  public string? Title { get; set; }
  public string? Organization { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // TODO: Add Relationships
  //public ICollection<Note> Notes {get; set; } = new List<Note>();
  public ICollection<EventContact> EventContacts { get; set; } = new List<EventContact>();
}