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
    public Guid? ContactId { get; set; }
    public Guid? EventId { get; set; }
  }
}