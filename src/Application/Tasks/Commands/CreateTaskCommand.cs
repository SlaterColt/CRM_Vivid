// src/Application/Tasks/Commands/CreateTaskCommand.cs
using CRM_Vivid.Core.Enum;
using MediatR;

namespace CRM_Vivid.Application.Tasks.Commands
{
  public class CreateTaskCommand : IRequest<Guid>
  {
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Core.Enum.TaskStatus Status { get; set; } = Core.Enum.TaskStatus.NotStarted;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? EventId { get; set; }
    // NEW: Add VendorId to allow linking tasks to vendors
    public Guid? VendorId { get; set; }
  }
}