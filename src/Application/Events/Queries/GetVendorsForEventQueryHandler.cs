// FILE: src/Application/Events/Queries/GetVendorsForEventQueryHandler.cs (NEW FILE)
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Events.Queries;

// Query is defined to fetch vendors for a specific event
public class GetVendorsForEventQuery : IRequest<List<VendorDto>>
{
  public Guid EventId { get; set; }
}

public class GetVendorsForEventQueryHandler : IRequestHandler<GetVendorsForEventQuery, List<VendorDto>>
{
  private readonly IApplicationDbContext _context;

  public GetVendorsForEventQueryHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<List<VendorDto>> Handle(GetVendorsForEventQuery request, CancellationToken cancellationToken)
  {
    // Select vendors linked to the event, projecting the Role from the join table
    return await _context.EventVendors
        .Where(ev => ev.EventId == request.EventId)
        .Select(ev => new VendorDto
        {
          Id = ev.Vendor!.Id,
          Name = ev.Vendor.Name,
          PhoneNumber = ev.Vendor.PhoneNumber,
          Email = ev.Vendor.Email,
          // NOTE: We rely on AutoMapper/ToString() conventions for enums here,
          // or ensure explicit mapping in MappingProfile for ServiceType.
          ServiceType = ev.Vendor.ServiceType.ToString(),
          Attributes = ev.Vendor.Attributes,

          // CRITICAL: Inject the Role from the join entity (ev) into the DTO
          Role = ev.Role
        })
        .ToListAsync(cancellationToken);
  }
}