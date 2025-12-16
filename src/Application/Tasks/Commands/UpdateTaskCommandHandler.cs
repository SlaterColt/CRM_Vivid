// FILE: src/Application/Tasks/Commands/UpdateTaskCommandHandler.cs (MODIFIED)
using AutoMapper;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CRM_Vivid.Core.Entities; // Required for entity lookup

namespace CRM_Vivid.Application.Tasks.Commands
{
  public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateTaskCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
      var entity = await _context.Tasks.FindAsync(new object[] { request.Id }, cancellationToken);

      if (entity == null)
      {
        throw new NotFoundException(nameof(Core.Entities.Task), request.Id);
      }

      // --- PHASE 30: HARDENED RESOLUTION LOGIC ---

      // Helper function to handle lookups
      Func<string, Type, Guid?> resolveId = (nameOrEmail, entityType) =>
      {
        if (string.IsNullOrWhiteSpace(nameOrEmail)) return null;

        if (entityType == typeof(Contact))
        {
          var contact = _context.Contacts.AsNoTracking().FirstOrDefault(c => c.Email == nameOrEmail);
          if (contact == null) throw new NotFoundException(nameof(Contact), nameOrEmail);
          return contact.Id;
        }
        if (entityType == typeof(Vendor))
        {
          var vendor = _context.Vendors.AsNoTracking().FirstOrDefault(v => v.Name == nameOrEmail);
          if (vendor == null) throw new NotFoundException(nameof(Vendor), nameOrEmail);
          return vendor.Id;
        }
        if (entityType == typeof(Event))
        {
          var eventEntity = _context.Events.AsNoTracking().FirstOrDefault(e => e.Name == nameOrEmail);
          if (eventEntity == null) throw new NotFoundException(nameof(Event), nameOrEmail);
          return eventEntity.Id;
        }
        return null;
      };

      // 1. Resolve ContactId via Email
      // Only perform lookup if ContactId is null AND ContactEmail is provided
      if (request.ContactId == null && !string.IsNullOrWhiteSpace(request.ContactEmail))
      {
        request.ContactId = resolveId(request.ContactEmail, typeof(Contact));
      }

      // 2. Resolve VendorId via Name
      if (request.VendorId == null && !string.IsNullOrWhiteSpace(request.VendorName))
      {
        request.VendorId = resolveId(request.VendorName, typeof(Vendor));
      }

      // 3. Resolve EventId via Name
      if (request.EventId == null && !string.IsNullOrWhiteSpace(request.EventName))
      {
        request.EventId = resolveId(request.EventName, typeof(Event));
      }

      // --- End Hardening ---

      _mapper.Map(request, entity);

      // Per Rule #2: Use UTC
      if (request.DueDate.HasValue)
      {
        entity.DueDate = request.DueDate.Value.ToUniversalTime();
      }

      // The update command doesn't implicitly clear GUIDs, so we map them manually 
      // from the potentially resolved/nulled request.
      entity.ContactId = request.ContactId;
      entity.EventId = request.EventId;
      entity.VendorId = request.VendorId;

      entity.UpdatedAt = DateTime.UtcNow;

      _context.Tasks.Update(entity);
      await _context.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}