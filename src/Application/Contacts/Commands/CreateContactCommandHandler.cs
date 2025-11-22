using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Features.Contacts.Commands;
using CRM_Vivid.Core.Entities;
using MediatR;

namespace CRM_Vivid.Application.Contacts.Commands;

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Guid>
{
  private readonly IApplicationDbContext _context;

  public CreateContactCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<Guid> Handle(CreateContactCommand request, CancellationToken cancellationToken)
  {
    var entity = new Contact
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      Email = request.Email,
      PhoneNumber = request.PhoneNumber,
      Organization = request.Organization,
      Title = request.Title,

      // --- NEW: Map Pipeline Fields ---
      Stage = request.Stage,
      ConnectionStatus = request.ConnectionStatus,
      Source = request.Source,

      // Set initial tracking
      IsLead = request.Stage != Core.Enum.LeadStage.Won,
      FollowUpCount = 0,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _context.Contacts.Add(entity);
    await _context.SaveChangesAsync(cancellationToken);

    return entity.Id;
  }
}