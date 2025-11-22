// src/Application/Events/Commands/DeleteEventCommand.cs
using MediatR;

namespace CRM_Vivid.Application.Events.Commands;

public class DeleteEventCommand : IRequest
{
  public Guid Id { get; set; }
}