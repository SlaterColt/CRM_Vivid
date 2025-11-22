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
        .HasMaxLength(2000); // Set a reasonable max length

    builder.Property(n => n.CreatedAt)
        .IsRequired();

    // --- Polymorphic Relationships ---
    // A Note can belong to one Contact, but a Contact can have many Notes
    // (This configuration defines the relationship from the 'Note' side)

    builder.HasOne(n => n.Contact)
        .WithMany() // We don't need to define the ICollection on Contact.cs
        .HasForeignKey(n => n.ContactId)
        .OnDelete(DeleteBehavior.Cascade); // Deleting a contact deletes its notes

    builder.HasOne(n => n.Event)
        .WithMany()
        .HasForeignKey(n => n.EventId)
        .OnDelete(DeleteBehavior.Cascade); // Deleting an event deletes its notes

    builder.HasOne(n => n.Task)
        .WithMany()
        .HasForeignKey(n => n.TaskId)
        .OnDelete(DeleteBehavior.Cascade); // Deleting a task deletes its notes

    builder.HasOne(n => n.Vendor)
        .WithMany()
        .HasForeignKey(n => n.VendorId)
        .OnDelete(DeleteBehavior.Cascade); // Deleting a vendor deletes its notes
  }
}