// FILE: src/Application/Tasks/Commands/CreateTaskCommandHandler.cs
using AutoMapper;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore; // Required for FirstOrDefaultAsync

namespace CRM_Vivid.Application.Tasks.Commands;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;
  private readonly ICurrentUserService _currentUserService;

  public CreateTaskCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
  {
    _context = context;
    _mapper = mapper;
    _currentUserService = currentUserService;
  }

  public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
  {
    // --- PHASE 29: Resolve IDs from String Lookups ---

    // 1. Resolve ContactId via Email
    if (request.ContactId == null && !string.IsNullOrWhiteSpace(request.ContactEmail))
    {
      var contact = await _context.Contacts
          .AsNoTracking()
          .FirstOrDefaultAsync(c => c.Email == request.ContactEmail, cancellationToken);
      request.ContactId = contact?.Id;
    }

    // 2. Resolve VendorId via Name
    if (request.VendorId == null && !string.IsNullOrWhiteSpace(request.VendorName))
    {
      var vendor = await _context.Vendors
          .AsNoTracking()
          .FirstOrDefaultAsync(v => v.Name == request.VendorName, cancellationToken);
      request.VendorId = vendor?.Id;
    }

    // 3. Resolve EventId via Name
    if (request.EventId == null && !string.IsNullOrWhiteSpace(request.EventName))
    {
      var eventEntity = await _context.Events
          .AsNoTracking()
          .FirstOrDefaultAsync(e => e.Name == request.EventName, cancellationToken);
      request.EventId = eventEntity?.Id;
    }

    // --- End Resolution ---

    var entity = _mapper.Map<Core.Entities.Task>(request);

    // --- PHASE 37: ASSIGN OWNER ---
    entity.CreatedByUserId = _currentUserService.CurrentUserId;

    // Per Rule #2: Use UTC
    if (request.DueDate.HasValue)
    {
      entity.DueDate = request.DueDate.Value.ToUniversalTime();
    }

    entity.CreatedAt = DateTime.UtcNow;

    await _context.Tasks.AddAsync(entity, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);

    return entity.Id;
  }
}