using CRM_Vivid.Core.Enum;
using MediatR;

namespace CRM_Vivid.Application.Contacts.Commands;

// This command implements IRequest<Unit>, which means it will
// execute and return 'nothing' (a 'Unit' value) upon success.
public class UpdateContactCommand : IRequest<Unit>
{
  // We need the Id to know WHICH contact to update.
  public Guid Id { get; set; }

  // The rest of the fields are the new values.
  public string FirstName { get; set; } = string.Empty;
  public string? LastName { get; set; }
  public string Email { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; }
  public string? Title { get; set; }
  public string? Organization { get; set; }

  public LeadStage Stage { get; init; }
  public ConnectionStatus ConnectionStatus { get; init; }
  public string? Source { get; init; }

  // Explicit action to signal a new follow-up (Diara's "1st, 2nd, 3rd follow ups")
  public bool IncrementFollowUpCount { get; init; } = false;
}