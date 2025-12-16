using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations
{
  public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
  {
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
      builder.HasKey(e => e.Id);

      builder.Property(e => e.Description)
          .IsRequired()
          .HasMaxLength(200);

      // Money Precision
      builder.Property(e => e.Amount)
          .HasPrecision(18, 2);

      // Relationship: Budget -> Expenses
      builder.HasOne(e => e.Budget)
          .WithMany(b => b.Expenses)
          .HasForeignKey(e => e.BudgetId)
          .OnDelete(DeleteBehavior.Cascade);

      // Relationship: Vendor (Optional)
      // If Vendor is deleted, keep the expense history but null the reference.
      builder.HasOne(e => e.Vendor)
          .WithMany(v => v.Expenses)
          .HasForeignKey(e => e.VendorId)
          .OnDelete(DeleteBehavior.SetNull);

      // Relationship: Document/Invoice (Optional)
      // Documents have 'int' PK.
      builder.HasOne(e => e.LinkedDocument)
          .WithMany()
          .HasForeignKey(e => e.LinkedDocumentId)
          .OnDelete(DeleteBehavior.SetNull);
    }
  }
}