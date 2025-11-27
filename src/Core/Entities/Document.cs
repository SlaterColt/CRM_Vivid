using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Protocol 10: The Task Alias

namespace CRM_Vivid.Core.Entities
{
  [Table("Documents")]
  public class Document
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public required string FileName { get; set; }

    [Required]
    [MaxLength(255)]
    public required string StoredFileName { get; set; }

    [Required]
    [MaxLength(100)]
    public required string ContentType { get; set; }

    public long Size { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Guid RelatedEntityId { get; set; }

    [Required]
    [MaxLength(50)]
    public required string RelatedEntityType { get; set; }

    // --- NEW: The Librarian ---
    [MaxLength(50)]
    public string Category { get; set; } = "General"; // Invoice, Contract, Rider, etc.
  }
}