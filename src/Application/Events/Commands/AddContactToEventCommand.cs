// src/Application/Events/Commands/AddContactToEventCommand.cs
using MediatR;

namespace CRM_Vivid.Application.Events.Commands;

public class AddContactToEventCommand : IRequest
{
  public Guid EventId { get; set; }
  public Guid ContactId { get; set; }
  public string? Role { get; set; } // e.g., "Guest", "Performer", "Staff"
}