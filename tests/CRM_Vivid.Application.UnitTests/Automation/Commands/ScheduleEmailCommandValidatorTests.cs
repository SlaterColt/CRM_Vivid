using CRM_Vivid.Application.Automation.Commands;
using FluentValidation.TestHelper;
using Xunit;

namespace CRM_Vivid.Application.UnitTests.Automation.Commands;

public class ScheduleEmailCommandValidatorTests
{
  private readonly ScheduleEmailCommandValidator _validator;

  public ScheduleEmailCommandValidatorTests()
  {
    _validator = new ScheduleEmailCommandValidator();
  }

  [Fact]
  public void Should_Have_Error_When_ContactId_Is_Empty()
  {
    var command = new ScheduleEmailCommand(Guid.Empty, Guid.NewGuid(), null);
    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.ContactId);
  }

  [Fact]
  public void Should_Have_Error_When_TemplateId_Is_Empty()
  {
    var command = new ScheduleEmailCommand(Guid.NewGuid(), Guid.Empty, null);
    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.TemplateId);
  }

  [Fact]
  public void Should_Have_Error_When_SendAt_Is_In_The_Past()
  {
    var pastDate = DateTime.UtcNow.AddMinutes(-5);
    var command = new ScheduleEmailCommand(Guid.NewGuid(), Guid.NewGuid(), pastDate);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.SendAt)
          .WithErrorMessage("Scheduled time must be in the future.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_SendAt_Is_In_The_Future()
  {
    var futureDate = DateTime.UtcNow.AddMinutes(10);
    var command = new ScheduleEmailCommand(Guid.NewGuid(), Guid.NewGuid(), futureDate);

    var result = _validator.TestValidate(command);

    result.ShouldNotHaveValidationErrorFor(x => x.SendAt);
  }

  [Fact]
  public void Should_Not_Have_Error_When_SendAt_Is_Null()
  {
    // Null means "Send Immediately"
    var command = new ScheduleEmailCommand(Guid.NewGuid(), Guid.NewGuid(), null);

    var result = _validator.TestValidate(command);

    result.ShouldNotHaveValidationErrorFor(x => x.SendAt);
  }
}