using FluentValidation;

namespace CRM_Vivid.Application.Notes.Validators;

public class UpdateNoteCommandValidator : AbstractValidator<Commands.UpdateNoteCommand>
{
  public UpdateNoteCommandValidator()
  {
    RuleFor(v => v.Content)
        .NotEmpty().WithMessage("Content is required.")
        .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters.");
  }
}