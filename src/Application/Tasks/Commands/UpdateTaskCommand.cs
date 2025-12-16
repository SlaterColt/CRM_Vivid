// FILE: src/Application/Tasks/Commands/UpdateTaskCommand.cs (MODIFIED)
using CRM_Vivid.Core.Enum;
using MediatR;

namespace CRM_Vivid.Application.Tasks.Commands
{
  public class UpdateTaskCommand : IRequest<Unit>
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Core.Enum.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }

    // Existing GUID Foreign Keys
    public Guid? ContactId { get; set; }
    public Guid? EventId { get; set; }
    public Guid? VendorId { get; set; } // NOTE: Assumed presence from previous phase work

    // --- NEW: PHASE 30 STRING-BASED RESOLUTION FIELDS (Optional) ---
    public string? ContactEmail { get; set; }
    public string? VendorName { get; set; }
    public string? EventName { get; set; }
  }
}