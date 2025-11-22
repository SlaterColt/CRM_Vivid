using CRM_Vivid.Core.Entities;
using CRM_Vivid.Core.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
  public void Configure(EntityTypeBuilder<Vendor> builder)
  {
    builder.HasKey(v => v.Id);

    builder.Property(v => v.Name)
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(v => v.PhoneNumber)
        .HasMaxLength(20);

    builder.Property(v => v.Email)
        .HasMaxLength(100);

    // This is the implementation of Lesson 6
    builder.Property(v => v.ServiceType)
        .HasConversion<string>()
        .HasMaxLength(50);
  }
}