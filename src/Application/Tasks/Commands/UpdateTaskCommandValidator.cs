// FILE: src/Application/Tasks/Commands/UpdateTaskCommandValidator.cs
using FluentValidation;

namespace CRM_Vivid.Application.Tasks.Commands
{
  public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
  {
    public UpdateTaskCommandValidator()
    {
      RuleFor(v => v.Id).NotEmpty();
      RuleFor(v => v.Title)
          .NotEmpty().WithMessage("Title is required.")
          .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
      RuleFor(v => v.Description)
          .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters.");

      // --- PHASE 30: ENFORCE ID/STRING MUTUAL EXCLUSION ---

      // Must not provide both ID and name/email for the same entity
      RuleFor(v => v.ContactId)
          .Empty().When(v => !string.IsNullOrWhiteSpace(v.ContactEmail))
          .WithMessage("Cannot provide both Contact ID and Contact Email.");

      RuleFor(v => v.EventId)
          .Empty().When(v => !string.IsNullOrWhiteSpace(v.EventName))
          .WithMessage("Cannot provide both Event ID and Event Name.");

      RuleFor(v => v.VendorId)
          .Empty().When(v => !string.IsNullOrWhiteSpace(v.VendorName))
          .WithMessage("Cannot provide both Vendor ID and Vendor Name.");

      // NOTE: We don't enforce mandatory linkage on UPDATE because a user may only be updating the Title or DueDate.
    }
  }
}