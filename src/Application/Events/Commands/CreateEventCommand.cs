using MediatR;

namespace CRM_Vivid.Application.Events.Commands
{
  public record CreateEventCommand : IRequest<Guid>
  {
    public string Name { get; init; } = string.Empty;
    public DateTime StartDateTime { get; init; }
    public DateTime EndDateTime { get; init; }
    public bool IsPublic { get; init; }
    public string? Description { get; init; }
    public string? Location { get; init; }
  }
}