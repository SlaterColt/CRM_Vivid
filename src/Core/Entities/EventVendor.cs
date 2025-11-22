// src/Core/Entities/EventVendor.cs
using System;

namespace CRM_Vivid.Core.Entities
{
  public class EventVendor
  {
    public Guid Id { get; set; }

    // Foreign Key to Event
    public Guid EventId { get; set; }
    public Event? Event { get; set; } // FIX: Marked as nullable (CS8618)

    // Foreign Key to Vendor
    public Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; } // FIX: Marked as nullable (CS8618)
  }
}