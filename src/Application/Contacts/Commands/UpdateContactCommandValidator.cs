using CRM_Vivid.Core.Enum;
using FluentValidation;
using System;

namespace CRM_Vivid.Application.Contacts.Commands;

public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
  public UpdateContactCommandValidator()
  {
    RuleFor(v => v.Id).NotEmpty().WithMessage("Contact ID is required for update.");
    RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
    RuleFor(v => v.Email).NotEmpty().EmailAddress().MaximumLength(200);

    // Validation for NEW Pipeline Fields
    RuleFor(v => v.Stage).IsInEnum().WithMessage("Invalid Lead Stage value.");
    RuleFor(v => v.ConnectionStatus).IsInEnum().WithMessage("Invalid Connection Status value.");
    RuleFor(v => v.Source).MaximumLength(100).When(v => v.Source != null);
  }
}