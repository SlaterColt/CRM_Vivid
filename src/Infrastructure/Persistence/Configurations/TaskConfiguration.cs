using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations
{
  public class TaskConfiguration : IEntityTypeConfiguration<Core.Entities.Task>
  {
    public void Configure(EntityTypeBuilder<Core.Entities.Task> builder)
    {
      builder.HasKey(t => t.Id);

      builder.Property(t => t.Title)
          .IsRequired()
          .HasMaxLength(200);

      builder.Property(t => t.Description)
          .HasMaxLength(5000);

      // Configure Enum to string conversion for the database
      builder.Property(t => t.Status)
          .HasConversion(
              v => v.ToString(),
              v => Enum.Parse<Core.Enum.TaskStatus>(v));

      builder.Property(t => t.Priority)
          .HasConversion(
              v => v.ToString(),
              v => Enum.Parse<Core.Enum.TaskPriority>(v));

      // Configure optional relationship with Contact
      builder.HasOne(t => t.Contact)
          .WithMany() // Assuming Contact doesn't need a list of Tasks
          .HasForeignKey(t => t.ContactId)
          .OnDelete(DeleteBehavior.SetNull) // Deleting a Contact sets Task.ContactId to null
          .IsRequired(false);

      // Configure optional relationship with Event
      builder.HasOne(t => t.Event)
          .WithMany() // Assuming Event doesn't need a list of Tasks
          .HasForeignKey(t => t.EventId)
          .OnDelete(DeleteBehavior.SetNull) // Deleting an Event sets Task.EventId to null
          .IsRequired(false);
    }
  }
}