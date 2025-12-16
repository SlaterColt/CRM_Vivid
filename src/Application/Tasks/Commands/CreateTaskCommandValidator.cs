// FILE: src/Application/Tasks/Commands/CreateTaskCommandValidator.cs
using FluentValidation;

namespace CRM_Vivid.Application.Tasks.Commands
{
  public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
  {
    public CreateTaskCommandValidator()
    {

      RuleFor(v => v.Title)
          .NotNull()
          .NotEmpty().WithMessage("Title is required.")
          .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

      RuleFor(v => v.Description)
          .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters.");


      // --- PHASE 29: ENFORCE ID/STRING MUTUAL EXCLUSION & LINKAGE ---

      // 1. Mutual Exclusion Rules
      RuleFor(v => v.ContactId)
          .Empty().When(v => !string.IsNullOrWhiteSpace(v.ContactEmail))
          .WithMessage("Cannot provide both Contact ID and Contact Email.");

      RuleFor(v => v.EventId)
          .Empty().When(v => !string.IsNullOrWhiteSpace(v.EventName))
          .WithMessage("Cannot provide both Event ID and Event Name.");

      RuleFor(v => v.VendorId)
          .Empty().When(v => !string.IsNullOrWhiteSpace(v.VendorName))
          .WithMessage("Cannot provide both Vendor ID and Vendor Name.");

      // 2. Mandatory Linkage Rule
      RuleFor(v => v)
          .Must(HaveAtLeastOneLink)
          .WithMessage("A task must be associated with at least one entity (Contact, Event, or Vendor) via ID or Name/Email.");
    }

    private bool HaveAtLeastOneLink(CreateTaskCommand command)
    {
      // Check if any of the GUID fields OR their corresponding string/email fields are populated
      return command.ContactId.HasValue || !string.IsNullOrWhiteSpace(command.ContactEmail) ||
             command.EventId.HasValue || !string.IsNullOrWhiteSpace(command.EventName) ||
             command.VendorId.HasValue || !string.IsNullOrWhiteSpace(command.VendorName);
    }
  }
}