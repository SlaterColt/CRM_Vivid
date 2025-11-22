// src/Application/Events/Queries/GetContactsForEventQuery.cs
using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Events.Queries
{
  public class GetContactsForEventQuery : IRequest<List<ContactDto>>
  {
    public Guid EventId { get; set; }
  }
}

