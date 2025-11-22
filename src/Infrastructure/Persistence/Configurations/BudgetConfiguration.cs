using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations
{
  public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
  {
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
      builder.HasKey(b => b.Id);

      // Money Precision (Critical for Financials)
      builder.Property(b => b.TotalAmount)
          .HasPrecision(18, 2);

      builder.Property(b => b.Currency)
          .HasMaxLength(3)
          .IsRequired();

      // 1:1 Relationship: Event -> Budget
      // If Event is deleted, the Budget is deleted.
      builder.HasOne(b => b.Event)
          .WithOne(e => e.Budget)
          .HasForeignKey<Budget>(b => b.EventId)
          .OnDelete(DeleteBehavior.Cascade);
    }
  }
}