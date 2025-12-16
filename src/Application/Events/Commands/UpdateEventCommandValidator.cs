// FILE: src/Application/Events/Commands/UpdateEventCommandValidator.cs
using FluentValidation;

namespace CRM_Vivid.Application.Events.Commands;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
  public UpdateEventCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Event ID is required for update.");
    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Event name is required.")
        .MaximumLength(200).WithMessage("Event name must not exceed 200 characters.");

    // --- PHASE 33 FIX: RELAX FUTURE DATE CONSTRAINT ON UPDATE ---
    // Constraint removed: GreaterThan(DateTime.UtcNow) 
    RuleFor(x => x.StartDateTime)
        .NotEmpty().WithMessage("Event start date/time is required.");

    RuleFor(x => x.EndDateTime)
        .NotEmpty().WithMessage("End date/time is required.")
        .GreaterThan(x => x.StartDateTime).WithMessage("End time must be after the start time.");
    // -------------------------------------------------------------

    RuleFor(x => x.Status)
        .NotEmpty().WithMessage("Status is required.");
  }
}