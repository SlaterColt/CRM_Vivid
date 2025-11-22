using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations
{
  public class EventContactConfiguration : IEntityTypeConfiguration<EventContact>
  {
    public void Configure(EntityTypeBuilder<EventContact> builder)
    {
      // Set primary key
      builder.HasKey(ec => new { ec.EventId, ec.ContactId });

      builder.Property(ec => ec.Role)
          .HasMaxLength(100);

      // Configure the many-to-many relationship

      // Relationship from Event -> EventContact
      builder.HasOne(ec => ec.Event)
          .WithMany(e => e.EventContacts) // Event has 'EventContacts' collection
          .HasForeignKey(ec => ec.EventId);
      // We can remove OnDelete cascade, EF's default is fine

      // Relationship from Contact -> EventContact
      builder.HasOne(ec => ec.Contact)
          .WithMany(c => c.EventContacts) // Contact has 'EventContacts' collection
          .HasForeignKey(ec => ec.ContactId);
    }
  }
}