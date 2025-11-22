using CRM_Vivid.Core.Enum;
using System;

namespace CRM_Vivid.Core.Entities
{
  public class Expense
  {
    public Guid Id { get; set; }

    // Parent Budget
    public Guid BudgetId { get; set; }
    public Budget? Budget { get; set; }

    // Optional Vendor Link
    public Guid? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DateIncurred { get; set; }

    public ExpenseCategory Category { get; set; }

    // Optional Link to an Invoice (Document uses int PK)
    public int? LinkedDocumentId { get; set; }
    public Document? LinkedDocument { get; set; }
  }
}