// src/Application/Events/Queries/GetEventByIdQueryHandler.cs
using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM_Vivid.Core.Entities;

namespace CRM_Vivid.Application.Events.Queries;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto?>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetEventByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<EventDto?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
  {
    var eventItem = await _context.Events
        .AsNoTracking() // Use AsNoTracking for read-only operations
        .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

    if (eventItem == null)
    {
      throw new NotFoundException(nameof(Event), request.Id);
    }

    return _mapper.Map<EventDto?>(eventItem);
  }
}