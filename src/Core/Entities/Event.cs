// src/Core/Entities/Event.cs (FULL FILE)
using CRM_Vivid.Core.Enums;
using System.Collections.Generic;

namespace CRM_Vivid.Core.Entities
{
  public class Event
  {
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public EventStatus Status { get; set; } // e.g., "Planned", "Confirmed", "Cancelled"

    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsPublic { get; set; }

    // For storing flexible data, like location, notes, etc. as JSON
    public string? Description { get; set; }

    // Navigation property for the linking table (EventContact)
    public ICollection<EventContact> EventContacts { get; set; } = new List<EventContact>();

    // NEW: Navigation property for the Vendor linking table (EventVendor)
    public ICollection<EventVendor> EventVendors { get; set; } = new List<EventVendor>();

    public string? Location { get; set; }
  }
}