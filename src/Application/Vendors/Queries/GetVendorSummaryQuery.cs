// FILE: src/Application/Vendors/Queries/GetVendorSummaryQuery.cs (NEW FILE)
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Vendors.Queries;

public record GetVendorSummaryQuery(Guid VendorId) : IRequest<VendorSummaryDto>;

public class GetVendorSummaryQueryHandler : IRequestHandler<GetVendorSummaryQuery, VendorSummaryDto>
{
  private readonly IApplicationDbContext _context;

  public GetVendorSummaryQueryHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<VendorSummaryDto> Handle(GetVendorSummaryQuery request, CancellationToken cancellationToken)
  {
    // 1. Fetch the base Vendor details and calculate metrics simultaneously
    var result = await _context.Vendors
        .Where(v => v.Id == request.VendorId)
        .Select(v => new VendorSummaryDto
        {
          VendorId = v.Id,
          VendorName = v.Name,

          // Calculate 1: Total Events Hired For (via EventVendor join table)
          TotalEventsHiredFor = v.EventVendors.Count(),

          // Calculate 2: Total Expenses Paid (via Expense table)
          TotalExpensesPaid = v.Expenses.Sum(e => (decimal?)e.Amount) ?? 0.00m
        })
        .SingleOrDefaultAsync(cancellationToken);

    if (result == null)
    {
      throw new NotFoundException(nameof(Vendor), request.VendorId);
    }

    return result;
  }
}