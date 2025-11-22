// src/Application/Contacts/Queries/GetEventsForContactQueryHandler.cs (FINAL CORRECTED VERSION)

using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Ensure this is available if needed

namespace CRM_Vivid.Application.Contacts.Queries;

public record GetEventsForContactQuery : IRequest<List<EventDto>>
{
  public Guid ContactId { get; init; }
}

public class GetEventsForContactQueryHandler : IRequestHandler<GetEventsForContactQuery, List<EventDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetEventsForContactQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<List<EventDto>> Handle(GetEventsForContactQuery request, CancellationToken cancellationToken)
  {
    // FIX: Force materialization of the Event entities first (ToListAsync), 
    // then use the standard mapper (.Map) to convert the list to DTOs.
    // This guarantees the Event entity data is loaded before mapping occurs.

    var events = await _context.EventContacts
        .Where(ec => ec.ContactId == request.ContactId)
        .Select(ec => ec.Event)
        .ToListAsync(cancellationToken); // Materialize the list of Event entities here

    // Now map the materialized list of Event entities to EventDto
    return _mapper.Map<List<EventDto>>(events);
  }
}