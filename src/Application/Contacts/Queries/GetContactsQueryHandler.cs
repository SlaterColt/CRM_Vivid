// FILE: src/Application/Contacts/Queries/GetContactsQueryHandler.cs (MODIFIED)
using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Contacts.Queries;

public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, List<ContactDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetContactsQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<List<ContactDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
  {
    // --- PHASE 34 FIX: EXPLICITLY PROJECT ALL CONTACT FIELDS TO PREVENT 500 CRASH ---
    return await _context.Contacts
        .AsNoTracking()
        .Select(c => new ContactDto
        {
          Id = c.Id,
          FirstName = c.FirstName,
          LastName = c.LastName,
          Email = c.Email,
          PhoneNumber = c.PhoneNumber,
          Title = c.Title,
          Organization = c.Organization,

          // Phase 25 Fields (Must be projected)
          Stage = c.Stage,
          ConnectionStatus = c.ConnectionStatus,
          IsLead = c.IsLead,
          FollowUpCount = c.FollowUpCount,
          LastContactedAt = c.LastContactedAt,
          Source = c.Source,

          // Phase 34 Role field is intentionally left null here, as this is the global query,
          // where a Contact does not have a role. The event-specific query handles the Role.
          Role = null
        })
        .ToListAsync(cancellationToken);
    // ---------------------------------------------------------------------------------
  }
}