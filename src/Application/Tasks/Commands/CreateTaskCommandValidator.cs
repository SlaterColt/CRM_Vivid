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
    }
  }
}