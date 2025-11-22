using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Core.Enum;
using MediatR;

namespace CRM_Vivid.Application.Contacts.Commands;

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Unit>
{
  private readonly IApplicationDbContext _context;

  public UpdateContactCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<Unit> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
  {
    // 1. Find the existing contact by its ID
    var entity = await _context.Contacts.FindAsync(
        new object[] { request.Id },
        cancellationToken: cancellationToken);

    // 2. Add proper Exception Handling
    if (entity == null)
    {
      throw new NotFoundException(nameof(Contact), request.Id);
    }

    // 3. Map the standard values
    entity.FirstName = request.FirstName;
    entity.LastName = request.LastName;
    entity.Email = request.Email;
    entity.PhoneNumber = request.PhoneNumber;
    entity.Title = request.Title;
    entity.Organization = request.Organization;
    entity.UpdatedAt = DateTime.UtcNow; // Ensure timestamp is updated

    // 4. Map and Process Pipeline Fields (NEW LOGIC)
    entity.Stage = request.Stage;
    entity.ConnectionStatus = request.ConnectionStatus;
    entity.Source = request.Source;

    if (request.IncrementFollowUpCount)
    {
      // Diara's "1st, 2nd, 3rd follow ups" tracking
      entity.FollowUpCount++;
      entity.LastContactedAt = DateTime.UtcNow;
    }

    // Handle Lead/Client Transition based on Stage (High-Value Logic)
    if (request.Stage == LeadStage.Won)
    {
      entity.IsLead = false; // Convert from Lead back to Client
    }
    else if (request.Stage != LeadStage.Won && entity.Stage == LeadStage.Won)
    {
      // If they were 'Won' but are being moved back (e.g., to Negotiating), they are treated as a Lead again.
      entity.IsLead = true;
    }

    // 5. Save the changes
    await _context.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}