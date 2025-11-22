using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore; // <-- Make sure this is included

namespace CRM_Vivid.Application.Contacts.Queries;

public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, List<ContactDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetContactsQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<List<ContactDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
  {
    // Fetch the list of all contacts.
    // We use .AsNoTracking() as a performance optimization
    // because this is a read-only operation.
    return await _context.Contacts
        .ProjectTo<ContactDto>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellationToken);
  }
}