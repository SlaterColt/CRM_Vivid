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
}