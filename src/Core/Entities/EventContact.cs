// FILE: src/Core/Entities/EventContact.cs
namespace CRM_Vivid.Core.Entities
{
  public class EventContact
  {
    public string? Role { get; set; } // e.g., "Guest", "Speaker", "Host" 
    public Guid CreatedByUserId { get; set; } // NEW

    // Foreign keys and navigation properties
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
  }
}