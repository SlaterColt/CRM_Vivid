namespace CRM_Vivid.Application.Common.Models
{
  public class ExpenseDto
  {
    public Guid Id { get; set; }
    public Guid BudgetId { get; set; }

    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DateIncurred { get; set; }
    public string Category { get; set; } = string.Empty; // Mapped from Enum

    // Vendor Info (if linked)
    public Guid? VendorId { get; set; }
    public string? VendorName { get; set; }

    // Invoice Info (if linked)
    public int? LinkedDocumentId { get; set; }
    public string? LinkedDocumentName { get; set; }
  }
}