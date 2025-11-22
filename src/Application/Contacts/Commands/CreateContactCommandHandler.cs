using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using CRM_Vivid.Application.Features.Contacts.Commands;

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
      Id = Guid.NewGuid(),
      FirstName = request.FirstName,
      LastName = request.LastName,
      Email = request.Email,
      PhoneNumber = request.PhoneNumber,
      Title = request.Title,
      Organization = request.Organization
    };

    _context.Contacts.Add(entity);

    await _context.SaveChangesAsync(cancellationToken);

    return entity.Id;
  }
}