using CRM_Vivid.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM_Vivid.Infrastructure.Persistence.Configurations
{
  public class DocumentConfiguration : IEntityTypeConfiguration<Document>
  {
    public void Configure(EntityTypeBuilder<Document> builder)
    {
      builder.HasKey(t => t.Id);

      builder.Property(t => t.FileName)
          .HasMaxLength(255)
          .IsRequired();

      builder.Property(t => t.StoredFileName)
          .HasMaxLength(255)
          .IsRequired();

      builder.Property(t => t.ContentType)
          .HasMaxLength(100)
          .IsRequired();

      builder.Property(t => t.RelatedEntityType)
          .HasMaxLength(50)
          .IsRequired();

      builder.HasIndex(t => new { t.RelatedEntityId, t.RelatedEntityType });
    }
  }
}