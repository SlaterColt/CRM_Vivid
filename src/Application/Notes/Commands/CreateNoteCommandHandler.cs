using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;

namespace CRM_Vivid.Application.Notes.Commands;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public CreateNoteCommandHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
  {
    var note = _mapper.Map<Note>(request);
    note.CreatedAt = DateTime.UtcNow;

    _context.Notes.Add(note);
    await _context.SaveChangesAsync(cancellationToken);

    return _mapper.Map<NoteDto>(note);
  }
}