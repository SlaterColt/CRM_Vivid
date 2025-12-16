// FILE: src/Core/Entities/EventVendor.cs
// src/Core/Entities/EventVendor.cs
namespace CRM_Vivid.Core.Entities
{
  public class EventVendor
  {
    public Guid Id { get; set; } // PK is GUID, preserved
    public Guid CreatedByUserId { get; set; }

    // Foreign Key to Event
    public Guid EventId { get; set; }
    public Event? Event { get; set; } // FIX: Marked as nullable (CS8618)
    // Foreign Key to Vendor
    public Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; } // FIX: Marked as nullable (CS8618)

    // --- NEW: PHASE 34 ROLE PROPERTY ---
    public string? Role { get; set; }
  }
}