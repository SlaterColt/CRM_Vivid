using CRM_Vivid.Application.Automation.Commands;
using CRM_Vivid.Application.Common.Models;
using FluentValidation.TestHelper;
using System;
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
  public void Should_Have_Error_When_Contact_Is_Null()
  {
    var command = new ScheduleEmailCommand
    {
      Contact = null!,
      Subject = "Test",
      TemplateContent = "Content",
      ScheduleTime = DateTime.UtcNow.AddHours(1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.Contact);
  }

  [Fact]
  public void Should_Have_Error_When_Email_Is_Empty()
  {
    var command = new ScheduleEmailCommand
    {
      Contact = new ContactDto { Email = "" },
      Subject = "Test",
      TemplateContent = "Content",
      ScheduleTime = DateTime.UtcNow.AddHours(1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.Contact.Email);
  }

  [Fact]
  public void Should_Have_Error_When_ScheduleTime_Is_Past()
  {
    var command = new ScheduleEmailCommand
    {
      Contact = new ContactDto { Email = "test@test.com" },
      Subject = "Test",
      TemplateContent = "Content",
      ScheduleTime = DateTime.UtcNow.AddHours(-1) // Past
    };

    var result = _validator.TestValidate(command);
    result.ShouldHaveValidationErrorFor(x => x.ScheduleTime);
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid()
  {
    var command = new ScheduleEmailCommand
    {
      Contact = new ContactDto { Email = "test@test.com", FirstName = "John" },
      Subject = "Valid Subject",
      TemplateContent = "Valid Content",
      ScheduleTime = DateTime.UtcNow.AddHours(1)
    };

    var result = _validator.TestValidate(command);
    result.ShouldNotHaveAnyValidationErrors();
  }
}