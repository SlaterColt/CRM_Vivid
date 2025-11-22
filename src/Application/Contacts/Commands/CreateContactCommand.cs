using CRM_Vivid.Core.Enum;
using MediatR;

namespace CRM_Vivid.Application.Features.Contacts.Commands;

public class CreateContactCommand : IRequest<Guid>
{
  public string FirstName { get; set; } = string.Empty;
  public string? LastName { get; set; }
  public string Email { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; }
  public string? Title { get; set; }
  public string? Organization { get; set; }

  public LeadStage Stage { get; init; } = Core.Enum.LeadStage.NewLead;
  public ConnectionStatus ConnectionStatus { get; init; } = Core.Enum.ConnectionStatus.NeedToMeet;
  public string? Source { get; init; }
}