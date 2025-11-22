using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
  public void Configure(EntityTypeBuilder<Template> builder)
  {
    builder.HasKey(t => t.Id);

    builder.Property(t => t.Name)
        .IsRequired()
        .HasMaxLength(200);

    builder.Property(t => t.Subject)
        .HasMaxLength(200);

    builder.Property(t => t.Content)
        .IsRequired();

    builder.Property(t => t.Type)
        .IsRequired()
        .HasConversion<string>();
  }
}