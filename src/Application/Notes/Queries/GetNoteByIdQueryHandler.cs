using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Notes.Queries;

public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetNoteByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<NoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
  {
    var note = await _context.Notes
        .AsNoTracking()
        .Where(n => n.Id == request.Id)
        .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
        .SingleOrDefaultAsync(cancellationToken);

    if (note == null)
    {
      throw new NotFoundException(nameof(Note), request.Id);
    }

    return note;
  }
}