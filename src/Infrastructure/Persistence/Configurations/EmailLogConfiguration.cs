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
          .IsRequired(false); // Explicitly nullable

      // Optional Relationship with Contact
      // We don't necessarily want a "EmailLogs" collection on the Contact entity 
      // clogging up the domain model unless we explicitly ask for it, 
      // so we configure the relationship from this side.
      builder.HasOne(e => e.Contact)
          .WithMany() // One Contact has many EmailLogs, but we haven't added the collection to Contact yet
          .HasForeignKey(e => e.ContactId)
          .OnDelete(DeleteBehavior.SetNull); // If contact is deleted, keep the log but null the ID
    }
  }
}