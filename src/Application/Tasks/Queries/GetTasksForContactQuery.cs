using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Tasks.Queries
{
  public class GetTasksForContactQuery : IRequest<IEnumerable<TaskDto>>
  {
    public Guid ContactId { get; set; }
  }
}