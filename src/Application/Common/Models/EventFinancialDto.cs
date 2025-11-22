using System;
using System.Collections.Generic;

namespace CRM_Vivid.Application.Common.Models
{
  public class EventFinancialsDto
  {
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;

    // Budget Settings
    public decimal BudgetTotal { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Notes { get; set; }

    // The Ledger
    public List<ExpenseDto> Expenses { get; set; } = new List<ExpenseDto>();

    // The Calculator (Computed Properties)
    public decimal TotalSpent { get; set; }
    public decimal RemainingBudget => BudgetTotal - TotalSpent;

    // Progress (0.0 to 1.0) for UI Bars
    public double BurnRate => BudgetTotal == 0 ? 0 : (double)(TotalSpent / BudgetTotal);
  }
}