using CRM_Vivid.Application.Common.Models;
using MediatR;
using System;

namespace CRM_Vivid.Application.Financials.Queries
{
  public record GetEventFinancialsQuery(Guid EventId) : IRequest<EventFinancialsDto>;
}