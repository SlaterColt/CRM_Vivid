// FILE: src/Core/Entities/Vendor.cs
// src/Core/Entities/Vendor.cs (FULL FILE)
using CRM_Vivid.Core.Enum;

namespace CRM_Vivid.Core.Entities;

public class Vendor : IUserScopedEntity
{
  public Guid Id { get; set; }
  public string Name
  {
    get; set;
  } = string.Empty;
  public string? PhoneNumber { get; set; }
  public string? Email
  {
    get; set;
  }
  public VendorType ServiceType { get; set; }

  // --- NEW: FLEXIBLE ATTRIBUTES (JSONB) ---
  public string? Attributes { get; set; }

  public Guid CreatedByUserId { get; set; }

  // NEW: Navigation property for Events via the join table
  public ICollection<EventVendor> EventVendors
  {
    get;
    set;
  } = new List<EventVendor>();

  public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}