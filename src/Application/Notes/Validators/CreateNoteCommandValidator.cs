using FluentValidation;

namespace CRM_Vivid.Application.Notes.Validators;

public class CreateNoteCommandValidator : AbstractValidator<Commands.CreateNoteCommand>
{
  public CreateNoteCommandValidator()
  {
    RuleFor(v => v.Content)
        .NotEmpty().WithMessage("Content is required.")
        .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters.");

    // --- LESSON 12 IMPLEMENTATION ---
    RuleFor(v => v)
        .Must(HaveAtLeastOneForeignKey)
        .WithMessage("A note must be associated with at least one entity (Contact, Event, Task, or Vendor).");
  }

  private bool HaveAtLeastOneForeignKey(Commands.CreateNoteCommand command)
  {
    return command.ContactId.HasValue ||
           command.EventId.HasValue ||
           command.TaskId.HasValue ||
           command.VendorId.HasValue;
  }
}