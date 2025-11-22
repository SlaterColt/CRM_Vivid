// src/Core/Entities/Task.cs
using CRM_Vivid.Core.Enum;
using System;

namespace CRM_Vivid.Core.Entities
{
  public class Task
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Enum.TaskStatus Status { get; set; } = Enum.TaskStatus.NotStarted;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign Keys
    public Guid? ContactId { get; set; }
    public Guid? EventId { get; set; }

    // NEW: Foreign Key for Vendor
    public Guid? VendorId { get; set; }

    // Navigation Properties
    public Contact? Contact { get; set; }
    public Event? Event { get; set; }

    // NEW: Navigation Property for Vendor
    public Vendor? Vendor { get; set; }
  }
}