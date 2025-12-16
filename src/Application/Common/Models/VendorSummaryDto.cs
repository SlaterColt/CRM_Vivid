// FILE: src/Application/Common/Models/VendorSummaryDto.cs (NEW FILE)
namespace CRM_Vivid.Application.Common.Models;

public class VendorSummaryDto
{
  public Guid VendorId { get; set; }
  public string VendorName { get; set; } = string.Empty;

  // Phase 35 Reporting Metrics
  public int TotalEventsHiredFor { get; set; }
  public decimal TotalExpensesPaid { get; set; } // Sum of all expenses linked to this vendor
}