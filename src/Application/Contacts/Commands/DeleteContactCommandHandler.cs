using CRM_Vivid.Application.Common.Interfaces;
using MediatR;

namespace CRM_Vivid.Application.Contacts.Commands;

public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, Unit>
{
  private readonly IApplicationDbContext _context;

  public DeleteContactCommandHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<Unit> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
  {
    // 1. Find the existing contact by its ID
    var entity = await _context.Contacts.FindAsync(
        new object[] { request.Id },
        cancellationToken: cancellationToken);

    // 2. If it exists, remove it
    if (entity != null)
    {
      _context.Contacts.Remove(entity);
      await _context.SaveChangesAsync(cancellationToken);
    }

    // 3. If it didn't exist, we don't need to do anything.
    // In both cases, we return 'Unit.Value'
    return Unit.Value;
  }
}