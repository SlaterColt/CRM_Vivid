using FluentValidation;

namespace CRM_Vivid.Application.Automation.Commands;

public class ScheduleEmailCommandValidator : AbstractValidator<ScheduleEmailCommand>
{
  public ScheduleEmailCommandValidator()
  {
    RuleFor(x => x.ContactId)
        .NotEmpty().WithMessage("Contact ID is required.");

    RuleFor(x => x.TemplateId)
        .NotEmpty().WithMessage("Template ID is required.");

    RuleFor(x => x.SendAt)
        .Must(BeInTheFuture).When(x => x.SendAt.HasValue)
        .WithMessage("Scheduled time must be in the future.");
  }

  private bool BeInTheFuture(DateTime? date)
  {
    return date.HasValue && date.Value > DateTime.UtcNow;
  }
}