using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using MediatR;
using CRM_Vivid.Core.Entities;

namespace CRM_Vivid.Application.Contacts.Queries;

public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactDto?>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetContactByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<ContactDto?> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
  {
    var contact = await _context.Contacts.FindAsync(
      new object[] { request.Id },
      cancellationToken: cancellationToken);

    if (contact == null)
    {
      throw new NotFoundException(nameof(Contact), request.Id);
    }

    return _mapper.Map<ContactDto?>(contact);

  }
}