using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        // --- FIX: Explicitly map to Contact.Notes ---
        // This removes the ContactId1 shadow property warning.
        builder.HasOne(n => n.Contact)
            .WithMany(c => c.Notes) // <--- FIX: Pointing to the ICollection on Contact
            .HasForeignKey(n => n.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Polymorphic Relationships (Preserved) ---
        builder.HasOne(n => n.Event)
            .WithMany() // Since Event.cs doesn't have an ICollection<Note> yet
            .HasForeignKey(n => n.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Task)
            .WithMany()
            .HasForeignKey(n => n.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Vendor)
            .WithMany()
            .HasForeignKey(n => n.VendorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}