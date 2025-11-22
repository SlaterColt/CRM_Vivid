// src/Core/Entities/Vendor.cs (FULL FILE)
using CRM_Vivid.Core.Enum;
using System.Collections.Generic;

namespace CRM_Vivid.Core.Entities;

public class Vendor
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; }
  public string? Email { get; set; }
  public VendorType ServiceType { get; set; }

  // NEW: Navigation property for Events via the join table
  public ICollection<EventVendor> EventVendors { get; set; } = new List<EventVendor>();
}