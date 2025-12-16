// FILE: tests/CRM_Vivid.Application.UnitTests/Automation/Commands/ScheduleEmailCommandHandlerTests.cs (MODIFIED)
using CRM_Vivid.Application.Automation.Commands;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using Moq;
using Xunit;
using System; // Required for Guid

namespace CRM_Vivid.Application.UnitTests.Automation.Commands;

public class ScheduleEmailCommandHandlerTests
{
  private readonly Mock<ITemplateMerger> _mockMerger;
  private readonly Mock<IEmailSender> _mockEmailSender;
  private readonly Mock<IApplicationDbContext> _mockContext;
  private readonly ScheduleEmailCommandHandler _handler;

  public ScheduleEmailCommandHandlerTests()
  {
    _mockMerger = new Mock<ITemplateMerger>();
    _mockEmailSender = new Mock<IEmailSender>();
    _mockContext = new Mock<IApplicationDbContext>();

    _handler = new ScheduleEmailCommandHandler(
        _mockMerger.Object,
        _mockEmailSender.Object,
        _mockContext.Object);
  }

  [Fact]
  public async Task Handle_ShouldMergeAndSendEmail()
  {
    // Arrange
    var command = new ScheduleEmailCommand
    {
      Contact = new ContactDto { FirstName = "John", Email = "john@example.com" },
      TemplateContent = "Hello {FirstName}",
      Subject = "Test Subject"
    };

    _mockMerger.Setup(m => m.Merge(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
               .Returns("Hello John");

    // Act
    await _handler.Handle(command, CancellationToken.None);

    // Assert
    _mockMerger.Verify(m => m.Merge(command.TemplateContent, It.IsAny<Dictionary<string, string>>()), Times.Once);

    // --- FIX: Explicitly include all parameters in the Verify expression ---
    _mockEmailSender.Verify(e => e.SendEmailAsync(
        "john@example.com",
        "Test Subject",
        "Hello John",
        It.IsAny<Guid?>(), // Explicitly pass a matcher for the templateId
        It.IsAny<Guid?>()  // Explicitly pass a matcher for the eventId
        ), Times.Once);
    // ----------------------------------------------------------------------
  }
}