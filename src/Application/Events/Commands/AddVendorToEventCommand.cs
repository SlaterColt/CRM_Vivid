// src/Application/Events/Commands/AddVendorToEventCommand.cs
using MediatR;
using System;

namespace CRM_Vivid.Application.Events.Commands
{
  public class AddVendorToEventCommand : IRequest<Guid>
  {
    public Guid EventId { get; set; }
    public Guid VendorId { get; set; }
  }
}