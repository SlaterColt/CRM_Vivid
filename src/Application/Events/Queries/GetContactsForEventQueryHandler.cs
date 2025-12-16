// FILE: src/Application/Events/Queries/GetContactsForEventQueryHandler.cs (MODIFIED)
// src/Application/Events/Queries/GetContactsForEventQueryHandler.cs
using AutoMapper;
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
    // --- PHASE 34 FIX: PROJECTING ROLE FROM JOIN TABLE ---
    return await _context.EventContacts
        .Where(ec => ec.EventId == request.EventId)
        // Select the Contact entity, but project the Role from the join table
        .Select(ec => new ContactDto
        {
          Id = ec.Contact.Id,
          FirstName = ec.Contact.FirstName,
          LastName = ec.Contact.LastName,
          Email = ec.Contact.Email,
          PhoneNumber = ec.Contact.PhoneNumber,
          Title = ec.Contact.Title,
          Organization = ec.Contact.Organization,
          Stage = ec.Contact.Stage,
          ConnectionStatus = ec.Contact.ConnectionStatus,
          IsLead = ec.Contact.IsLead,
          FollowUpCount = ec.Contact.FollowUpCount,
          LastContactedAt = ec.Contact.LastContactedAt,
          Source = ec.Contact.Source,

          // CRITICAL: Inject the Role from the join entity (ec) into the DTO
          Role = ec.Role
        })
        .ToListAsync(cancellationToken);
    // -----------------------------------------------------------------
  }
}