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

            // FIX: Explicitly map relationship to Contact.Tasks
            builder.HasOne(t => t.Contact)
                .WithMany(c => c.Tasks) // <--- FIX: Pointing to the ICollection on Contact
                .HasForeignKey(t => t.ContactId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Configure optional relationship with Event (Preserved)
            builder.HasOne(t => t.Event)
                .WithMany()
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}