// FILE: src/Application/Common/Models/ContactDto.cs (MODIFIED)
using CRM_Vivid.Core.Enum; // Required for LeadStage and ConnectionStatus

namespace CRM_Vivid.Application.Common.Models
{
  public class ContactDto
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public string? Title { get; set; }
    public string? Organization { get; set; }

    // --- PHASE 25: LEAD MANAGEMENT FIELDS (Missing and causing CS0117) ---
    public LeadStage Stage { get; set; }
    public ConnectionStatus ConnectionStatus { get; set; }
    public bool IsLead { get; set; }
    public int FollowUpCount { get; set; }
    public DateTime? LastContactedAt { get; set; }
    public string? Source { get; set; }
    // ----------------------------------------------------------------------

    // --- PHASE 34: EVENT GRANULARITY ROLE ---
    public string? Role { get; set; } // Used when listing participants for an Event
  }
}