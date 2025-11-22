using FluentValidation;

namespace CRM_Vivid.Application.Events.Commands
{
  public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
  {
    public CreateEventCommandValidator()
    {
      RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Event name is required.")
          .MaximumLength(200).WithMessage("Event name must not exceed 200 characters.");

      RuleFor(x => x.StartDateTime)
          .NotEmpty().WithMessage("Event date is required.")
          .GreaterThan(DateTime.UtcNow).WithMessage("Event date must be in the future.");

      RuleFor(x => x.EndDateTime)
                .NotEmpty().WithMessage("End date/time is required.")
                .GreaterThan(x => x.StartDateTime).WithMessage("End time must be after the start time.");

      RuleFor(x => x.Location)
          .MaximumLength(300).WithMessage("Location must not exceed 300 characters.");

      RuleFor(x => x.Description)
        .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
  }
}