// src/Application/Events/Commands/UpdateEventCommand.cs
using MediatR;

namespace CRM_Vivid.Application.Events.Commands;

public class UpdateEventCommand : IRequest
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public DateTime StartDateTime { get; set; }
  public DateTime EndDateTime { get; set; }
  public string? Location { get; set; }
  public string? Description { get; set; }

  public bool IsPublic { get; set; }
  public string? Attributes { get; set; }
}