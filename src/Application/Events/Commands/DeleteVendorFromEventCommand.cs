// src/Application/Events/Commands/DeleteVendorFromEventCommand.cs
using MediatR;
using System;

namespace CRM_Vivid.Application.Events.Commands
{
  public class DeleteVendorFromEventCommand : IRequest
  {
    public Guid EventId { get; set; }
    public Guid VendorId { get; set; }
  }
}