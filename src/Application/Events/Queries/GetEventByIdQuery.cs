// src/Application/Events/Queries/GetEventByIdQuery.cs
using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Events.Queries;

public class GetEventByIdQuery : IRequest<EventDto?>
{
  public Guid Id { get; set; }
}