using CRM_Vivid.Core.Enum; // Uses the new file

namespace CRM_Vivid.Core.Entities;

public class Contact : IUserScopedEntity
{
  public Guid Id { get; set; } = Guid.NewGuid();

  public string FirstName { get; set; } = string.Empty;
  public string? LastName { get; set; }
  public string Email { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; }

  public string? Title { get; set; }
  public string? Organization { get; set; }

  // --- Timestamps (Preserved) ---
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // --- Pipeline & Logic (New) ---
  public ConnectionStatus ConnectionStatus { get; set; } // Renamed to match Enum file strictly
  public LeadStage Stage { get; set; } = LeadStage.NewLead; // New for Pipeline
  public bool IsLead { get; set; } // To distinguish Leads from confirmed Clients
  public int FollowUpCount { get; set; } // Track 1st, 2nd, 3rd follow up

  public DateTime? LastContactedAt { get; set; }
  public string? Source { get; set; } // e.g., "Intake Form", "Referral"

  // --- Relationships ---
  public ICollection<EventContact> EventContacts { get; set; } = new List<EventContact>();

  // Added these back to support "Notes tab" and "Tasks bar"
  public ICollection<Note> Notes { get; set; } = new List<Note>();
  public ICollection<Task> Tasks { get; set; } = new List<Task>();
  public ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();
  public Guid CreatedByUserId { get; set; }
}