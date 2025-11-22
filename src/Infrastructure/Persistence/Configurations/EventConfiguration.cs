using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations
{
  public class EventConfiguration : IEntityTypeConfiguration<Event>
  {
    public void Configure(EntityTypeBuilder<Event> builder)
    {
      // Set primary key
      builder.HasKey(e => e.Id);

      // Configure propertiess
      builder.Property(e => e.Name)
          .IsRequired()
          .HasMaxLength(256);

      builder.Property(e => e.Status)
          .IsRequired()
          .HasConversion<string>()
          .HasMaxLength(50);

      builder.Property(e => e.Location)
      .HasMaxLength(300);

      builder.Property(e => e.Description)
          .HasMaxLength(500);
      // This configures the navigation property.
      // EF Core will automatically handle the foreign key relationship
      // when we configure the EventContact side.

    }
  }
}