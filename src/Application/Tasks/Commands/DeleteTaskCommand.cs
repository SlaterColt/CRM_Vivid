using MediatR;

namespace CRM_Vivid.Application.Tasks.Commands
{
  public class DeleteTaskCommand : IRequest<Unit>
  {
    public Guid Id { get; set; }
  }
}