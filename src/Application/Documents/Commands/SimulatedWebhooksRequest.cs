using System.ComponentModel.DataAnnotations;

namespace CRM_Vivid.Application.Documents.Commands
{
  /// <summary>
  /// DTO representing a simulated contract provider webhook payload.
  /// </summary>
  public class SimulatedWebhookRequest
  {
    // DocumentId is used to look up the document in our system.
    [Required]
    public int DocumentId { get; set; }

    // The status reported by the external provider.
    // We will map this string to our internal ContractStatus enum.
    [Required]
    [MaxLength(20)]
    public required string Status { get; set; }

    // Security check: A secret key or token that the external system uses for validation.
    [Required]
    public required string WebhookSecret { get; set; }
  }
}