// src/Infrastructure/Persistence/Configurations/EventVendorConfiguration.cs
using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations
{
  public class EventVendorConfiguration : IEntityTypeConfiguration<EventVendor>
  {
    public void Configure(EntityTypeBuilder<EventVendor> builder)
    {
      // Set Id as the Primary Key
      builder.HasKey(ev => ev.Id);

      // Event relationship
      builder
          .HasOne(ev => ev.Event)
          // FIX: Used the correct navigation property name 'EventVendors'
          .WithMany(e => e.EventVendors)
          .HasForeignKey(ev => ev.EventId)
          .IsRequired();

      // Vendor relationship
      builder
          .HasOne(ev => ev.Vendor)
          // FIX: Used the correct navigation property name 'EventVendors'
          .WithMany(v => v.EventVendors)
          .HasForeignKey(ev => ev.VendorId)
          .IsRequired();

      // Unique constraint to prevent duplicate links: one vendor can only be linked to an event once.
      builder.HasIndex(ev => new { ev.EventId, ev.VendorId }).IsUnique();
    }
  }
}