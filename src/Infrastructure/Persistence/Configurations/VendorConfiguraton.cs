// FILE: src/Infrastructure/Persistence/Configurations/VendorConfiguraton.cs
using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; // NEW: Required for ValueConverter

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

        builder.Property(v => v.ServiceType)
            .HasConversion<string>()
            .HasMaxLength(50);

        // --- PHASE 32 FINAL FIX: Explicit Conversion + ColumnType + Nullability ---
        builder.Property(v => v.Attributes)
            .HasConversion(
                new ValueConverter<string?, string?>(
                    v => v, // C# string to DB string
                    v => v  // DB string back to C# string
                )
            )
            .HasColumnType("jsonb") // CRITICAL: Re-state the column type
            .IsRequired(false); // CRITICAL: Ensure nulls are handled correctly
    }
}