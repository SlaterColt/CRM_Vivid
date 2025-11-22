// src/Core/Entities/Event.cs
using CRM_Vivid.Core.Enums;
using System.Collections.Generic;

namespace CRM_Vivid.Core.Entities
{
  public class Event
  {
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public EventStatus Status { get; set; }

    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsPublic { get; set; }

    public string? Description { get; set; }

    public ICollection<EventContact> EventContacts { get; set; } = new List<EventContact>();

    public ICollection<EventVendor> EventVendors { get; set; } = new List<EventVendor>();

    // --- NEW: The Ledger ---
    // Navigation property for the Financial Budget (1:0..1)
    public Budget? Budget { get; set; }

    public string? Location { get; set; }
  }
}