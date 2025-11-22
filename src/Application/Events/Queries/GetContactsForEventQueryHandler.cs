// src/Application/Events/Queries/GetContactsForEventQueryHandler.cs
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Events.Queries;

public class GetContactsForEventQueryHandler : IRequestHandler<GetContactsForEventQuery, List<ContactDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetContactsForEventQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<List<ContactDto>> Handle(GetContactsForEventQuery request, CancellationToken cancellationToken)
  {
    return await _context.EventContacts
        .Where(ec => ec.EventId == request.EventId)
        .Select(ec => ec.Contact)
        .ProjectTo<ContactDto>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellationToken);
  }
}