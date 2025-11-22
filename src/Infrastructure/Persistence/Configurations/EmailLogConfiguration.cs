using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations
{
  public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
  {
    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
      builder.HasKey(e => e.Id);

      builder.Property(e => e.To)
          .IsRequired()
          .HasMaxLength(256);

      builder.Property(e => e.Subject)
          .IsRequired()
          .HasMaxLength(500);

      builder.Property(e => e.Body)
          .IsRequired();

      builder.Property(e => e.IsSuccess)
          .IsRequired();

      builder.Property(e => e.ErrorMessage)
          .IsRequired(false);

      // FIX: Explicitly map relationship to Contact.EmailLogs
      builder.HasOne(e => e.Contact)
          .WithMany(c => c.EmailLogs) // <--- FIX: Pointing to the ICollection on Contact
          .HasForeignKey(e => e.ContactId)
          .OnDelete(DeleteBehavior.SetNull);
    }
  }
}