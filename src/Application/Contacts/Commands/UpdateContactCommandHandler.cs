using CRM_Vivid.Application.Common.Interfaces;
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

    // 2. Check if it exists. If not, we're done.
    // (We can add custom exception handling later if we want)
    if (entity == null)
    {
      // You could throw a NotFoundException here
      return Unit.Value;
    }

    // 3. Map the new values from the request to the entity
    entity.FirstName = request.FirstName;
    entity.LastName = request.LastName;
    entity.Email = request.Email;
    entity.PhoneNumber = request.PhoneNumber;
    entity.Title = request.Title;
    entity.Organization = request.Organization;

    // Note: Your entity automatically updates 'UpdatedAt',
    // but if it didn't, you'd set it here:
    // entity.UpdatedAt = DateTime.UtcNow;

    // 4. Save the changes to the database
    await _context.SaveChangesAsync(cancellationToken);

    // 5. Return the 'Unit' value (void)
    return Unit.Value;
  }
}