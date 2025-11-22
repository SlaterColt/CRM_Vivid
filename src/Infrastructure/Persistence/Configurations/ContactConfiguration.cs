using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
  public void Configure(EntityTypeBuilder<Contact> builder)
  {
    builder.ToTable("Contacts");
    builder.HasKey(c => c.Id);

    // Critical: Prevent duplicate emails
    builder.HasIndex(c => c.Email).IsUnique();

    // New: Index for Pipeline Performance
    builder.HasIndex(c => c.IsLead);
    builder.HasIndex(c => c.Stage);

    builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
    builder.Property(c => c.LastName).HasMaxLength(100);
    builder.Property(c => c.Email).IsRequired().HasMaxLength(256);
    builder.Property(c => c.PhoneNumber).HasMaxLength(50);
    builder.Property(c => c.Title).HasMaxLength(100);
    builder.Property(c => c.Organization).HasMaxLength(100);

    // Preserved PostgreSQL specific timestamp logic
    builder.Property(c => c.CreatedAt)
        .IsRequired()
        .HasDefaultValueSql("now()");

    builder.Property(c => c.UpdatedAt)
        .IsRequired()
        .HasDefaultValueSql("now()");
  }
}