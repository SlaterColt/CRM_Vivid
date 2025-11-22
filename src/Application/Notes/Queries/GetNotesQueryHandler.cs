using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Notes.Queries;

public class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, IEnumerable<NoteDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetNotesQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<IEnumerable<NoteDto>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
  {
    var query = _context.Notes.AsNoTracking();

    if (request.ContactId.HasValue)
    {
      query = query.Where(n => n.ContactId == request.ContactId);
    }
    else if (request.EventId.HasValue)
    {
      query = query.Where(n => n.EventId == request.EventId);
    }
    else if (request.TaskId.HasValue)
    {
      query = query.Where(n => n.TaskId == request.TaskId);
    }
    else if (request.VendorId.HasValue)
    {
      query = query.Where(n => n.VendorId == request.VendorId);
    }

    // If no filter is provided, it returns all notes

    return await query
        .OrderByDescending(n => n.CreatedAt)
        .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellationToken);
  }
}