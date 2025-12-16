// FILE: src/Application/Notes/Validators/CreateNoteCommandValidator.cs
using FluentValidation;
using CRM_Vivid.Application.Notes.Commands; // Ensure correct namespace for command

namespace CRM_Vivid.Application.Notes.Validators;

public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
  public CreateNoteCommandValidator()
  {
    RuleFor(v => v.Content)
        .NotEmpty().WithMessage("Content is required.")
        .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters.");

    // --- PHASE 29: ENFORCE ID/NAME MUTUAL EXCLUSION & LINKAGE ---

    // Must not provide both ID and name/email/title for the same entity
    RuleFor(v => v.ContactId).Empty().When(v => !string.IsNullOrWhiteSpace(v.ContactEmail))
        .WithMessage("Cannot provide both ContactId and ContactEmail.");
    RuleFor(v => v.EventId).Empty().When(v => !string.IsNullOrWhiteSpace(v.EventName))
        .WithMessage("Cannot provide both EventId and EventName.");
    RuleFor(v => v.VendorId).Empty().When(v => !string.IsNullOrWhiteSpace(v.VendorName))
        .WithMessage("Cannot provide both VendorId and VendorName.");
    RuleFor(v => v.TaskId).Empty().When(v => !string.IsNullOrWhiteSpace(v.TaskTitle))
        .WithMessage("Cannot provide both TaskId and TaskTitle.");

    // Must link to at least one entity (GUID or resolved string)
    RuleFor(v => v)
        .Must(HaveAtLeastOneLink)
        .WithMessage("A note must be associated with at least one entity (Contact, Event, Task, or Vendor).");
  }

  private bool HaveAtLeastOneLink(CreateNoteCommand command)
  {
    return command.ContactId.HasValue || !string.IsNullOrWhiteSpace(command.ContactEmail) ||
           command.EventId.HasValue || !string.IsNullOrWhiteSpace(command.EventName) ||
           command.TaskId.HasValue || !string.IsNullOrWhiteSpace(command.TaskTitle) ||
           command.VendorId.HasValue || !string.IsNullOrWhiteSpace(command.VendorName);
  }
}