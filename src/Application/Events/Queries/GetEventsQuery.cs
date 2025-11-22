using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Events.Queries
{
  // This query will fetch all events, so it doesn't need any properties.
  // We're returning a list of Event entities directly for now.
  public record GetEventsQuery : IRequest<List<EventDto>>
  {

  }
}