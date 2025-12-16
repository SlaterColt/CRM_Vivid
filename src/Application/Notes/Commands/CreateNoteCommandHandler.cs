// FILE: src/Application/Notes/Commands/CreateNoteCommandHandler.cs (MODIFIED)
using AutoMapper;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM_Vivid.Application.Exceptions; // NEW: Ensure this is used

namespace CRM_Vivid.Application.Notes.Commands;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  private readonly ICurrentUserService _currentUserService;

  public CreateNoteCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
  {
    _context = context;
    _mapper = mapper;
    _currentUserService = currentUserService;
  }

  public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
  {
    // --- PHASE 30: HARDENED RESOLUTION LOGIC ---

    // 1. Resolve ContactId via Email
    if (request.ContactId == null && !string.IsNullOrWhiteSpace(request.ContactEmail))
    {
      var contact = await _context.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Email == request.ContactEmail, cancellationToken);
      if (contact == null) throw new NotFoundException(nameof(Contact), request.ContactEmail);
      request.ContactId = contact.Id;
    }

    // 2. Resolve VendorId via Name
    if (request.VendorId == null && !string.IsNullOrWhiteSpace(request.VendorName))
    {
      var vendor = await _context.Vendors.AsNoTracking().FirstOrDefaultAsync(v => v.Name == request.VendorName, cancellationToken);
      if (vendor == null) throw new NotFoundException(nameof(Vendor), request.VendorName);
      request.VendorId = vendor.Id;
    }

    // 3. Resolve EventId via Name
    if (request.EventId == null && !string.IsNullOrWhiteSpace(request.EventName))
    {
      var eventEntity = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Name == request.EventName, cancellationToken);
      if (eventEntity == null) throw new NotFoundException(nameof(Event), request.EventName);
      request.EventId = eventEntity.Id;
    }

    // 4. Resolve TaskId via Title
    if (request.TaskId == null && !string.IsNullOrWhiteSpace(request.TaskTitle))
    {
      var task = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Title == request.TaskTitle, cancellationToken);
      // NOTE: We allow non-unique titles, but fail if *nothing* is found.
      if (task == null) throw new NotFoundException(nameof(Core.Entities.Task), request.TaskTitle);
      request.TaskId = task.Id;
    }

    // --- End Hardening ---

    var note = _mapper.Map<Note>(request);

    // --- PHASE 37: ASSIGN OWNER ---
    note.CreatedByUserId = _currentUserService.CurrentUserId;

    note.CreatedAt = DateTime.UtcNow;

    _context.Notes.Add(note);
    await _context.SaveChangesAsync(cancellationToken);

    var createdNote = await _context.Notes
        .Include(n => n.Vendor)
        .FirstOrDefaultAsync(n => n.Id == note.Id, cancellationToken);

    return _mapper.Map<NoteDto>(createdNote);
  }
}