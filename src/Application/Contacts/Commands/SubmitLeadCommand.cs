// FILE: src/Application/Contacts/Commands/SubmitLeadCommand.cs (MODIFIED)

using CRM_Vivid.Core.Entities;
using CRM_Vivid.Core.Enum;
using MediatR;
using FluentValidation;
using CRM_Vivid.Application.Common.Interfaces; // ADDED

namespace CRM_Vivid.Application.Contacts.Commands;

// Command for public form submission
public record SubmitLeadCommand : IRequest<Guid>
{
  // Minimal required info from the external form
  public string FirstName { get; init; } = string.Empty;
  public string? LastName { get; init; }
  public string Email { get; init; } = string.Empty;
  public string? PhoneNumber { get; init; }
  public string? Organization { get; init; }
  public string? Source { get; init; } // e.g., "Website Form"
}

public class SubmitLeadCommandValidator : AbstractValidator<SubmitLeadCommand>
{
  public SubmitLeadCommandValidator()
  {
    RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
    RuleFor(v => v.Email).NotEmpty().EmailAddress().MaximumLength(200);
  }
}

public class SubmitLeadCommandHandler : IRequestHandler<SubmitLeadCommand, Guid>
{
  private readonly IApplicationDbContext _context;
  private readonly ICurrentUserService _currentUserService; // ADDED

  public SubmitLeadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) // MODIFIED
  {
    _context = context;
    _currentUserService = currentUserService; // ADDED
  }

  public async Task<Guid> Handle(SubmitLeadCommand request, CancellationToken cancellationToken)
  {
    var entity = new Contact
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      Email = request.Email,
      PhoneNumber = request.PhoneNumber,
      Organization = request.Organization,
      Source = request.Source,

      // --- Pipeline Defaults (Crucial) ---
      IsLead = true,
      Stage = LeadStage.NewLead,
      ConnectionStatus = ConnectionStatus.NeedToMeet, // New leads need introduction

      // --- PHASE 41 FIX: ASSIGN OWNER ID ---
      CreatedByUserId = _currentUserService.CurrentUserId,
      // ------------------------------------

      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _context.Contacts.Add(entity);
    await _context.SaveChangesAsync(cancellationToken);

    return entity.Id;
  }
}