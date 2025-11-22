using CRM_Vivid.Application.Features.Contacts.Commands;
using CRM_Vivid.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Contacts.Commands
{
  public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
  {

    private readonly IApplicationDbContext _context;
    public CreateContactCommandValidator(IApplicationDbContext context)
    {
      _context = context;

      RuleFor(x => x.FirstName)
          .NotEmpty().WithMessage("First name is required.")
          .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

      RuleFor(x => x.LastName)
          .NotEmpty().WithMessage("Last name is required.")
          .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required.")
          .EmailAddress().WithMessage("A valid email address is required.")
          .MustAsync(BeUniqueEmail).WithMessage("This email address is already in use.");

      RuleFor(x => x.PhoneNumber)
          .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");

      RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

      RuleFor(x => x.Organization)
          .MaximumLength(200).WithMessage("Organization must not exceed 200 characters.");

      RuleFor(v => v.Stage).IsInEnum().WithMessage("Invalid Lead Stage value.");
      RuleFor(v => v.ConnectionStatus).IsInEnum().WithMessage("Invalid Connection Status value.");
      RuleFor(v => v.Source).MaximumLength(100).When(v => v.Source != null);
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
      // Will return true if no contact exists with this email
      return await _context.Contacts
          .AllAsync(c => c.Email != email, cancellationToken);
    }
  }
}