using CRM_Vivid.Application.Templates.Commands;
using CRM_Vivid.Core.Enum;
using FluentValidation;

namespace CRM_Vivid.Application.Templates.Validators;

public class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
  public CreateTemplateCommandValidator()
  {
    RuleFor(v => v.Name)
        .NotEmpty().WithMessage("Name is required.")
        .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

    RuleFor(v => v.Content)
        .NotEmpty().WithMessage("Content is required.");

    RuleFor(v => v.Type)
        .NotEmpty().WithMessage("Type is required.")
        .IsEnumName(typeof(TemplateType), caseSensitive: false)
        .WithMessage("Type must be either 'Email' or 'SMS'.");
  }
}