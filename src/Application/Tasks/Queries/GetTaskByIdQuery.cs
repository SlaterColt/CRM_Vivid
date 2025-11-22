using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Tasks.Queries
{
  public class GetTaskByIdQuery : IRequest<TaskDto>
  {
    public Guid Id { get; set; }
  }
}