using FluentValidation;

namespace CRM_Vivid.Application.Automation.Commands;

public class ScheduleEmailCommandValidator : AbstractValidator<ScheduleEmailCommand>
{
  public ScheduleEmailCommandValidator()
  {
    RuleFor(v => v.Contact)
        .NotNull().WithMessage("Contact information is required.");

    RuleFor(v => v.Contact.Email)
        .NotEmpty().WithMessage("Contact email is required.")
        .EmailAddress().WithMessage("A valid email address is required.")
        .When(v => v.Contact != null);

    RuleFor(v => v.Subject)
        .NotEmpty().WithMessage("Subject is required.");

    RuleFor(v => v.TemplateContent)
        .NotEmpty().WithMessage("Template content is required.");

    RuleFor(v => v.ScheduleTime)
        .GreaterThan(DateTime.UtcNow).WithMessage("Schedule time must be in the future.");
  }
}