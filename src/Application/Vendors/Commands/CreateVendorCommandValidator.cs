using FluentValidation;
using CRM_Vivid.Core.Enum; // <-- 1. Import the Enum

namespace CRM_Vivid.Application.Vendors.Commands;

public class CreateVendorCommandValidator : AbstractValidator<CreateVendorCommand>
{
  public CreateVendorCommandValidator()
  {
    RuleFor(v => v.Name)
        .NotEmpty().WithMessage("Name is required.")
        .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

    RuleFor(v => v.Email)
        .EmailAddress().WithMessage("A valid email address is required.")
        .When(v => !string.IsNullOrEmpty(v.Email)); // Only validate if provided

    // --- 2. This is the FIX ---
    RuleFor(v => v.ServiceType)
        .NotEmpty().WithMessage("ServiceType is required.")
        .IsEnumName(typeof(VendorType), caseSensitive: false) // Validates the string
        .WithMessage("A valid ServiceType is required.");
  }
}