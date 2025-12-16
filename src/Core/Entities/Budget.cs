namespace CRM_Vivid.Core.Entities
{
  public class Budget
  {
    public Guid Id { get; set; }

    // Foreign Key to Event (1:1)
    public Guid EventId { get; set; }
    public Event? Event { get; set; }

    public decimal TotalAmount { get; set; } // The cap/goal
    public string Currency { get; set; } = "USD";

    public string? Notes { get; set; }

    // --- PHASE 26 ADDITION: Locking Mechanism ---
    public bool IsLocked { get; set; } = false; // NEW

    // Navigation to Line Items
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
  }
}