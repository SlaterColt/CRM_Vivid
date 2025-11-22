using AutoMapper;
using CRM_Vivid.Application.Automation.Commands;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Infrastructure.Persistence;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace CRM_Vivid.Application.UnitTests.Automation.Commands;

public class ScheduleEmailCommandHandlerTests : IDisposable
{
  private readonly ApplicationDbContext _context;
  private readonly Mock<ITemplateMerger> _mergerMock;
  private readonly Mock<IBackgroundJobClient> _jobClientMock;
  private readonly Mock<IMapper> _mapperMock;
  private readonly ScheduleEmailCommandHandler _handler;

  public ScheduleEmailCommandHandlerTests()
  {
    // 1. Setup InMemory Database
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    _context = new ApplicationDbContext(options);

    // 2. Setup Mocks
    _mergerMock = new Mock<ITemplateMerger>();
    _jobClientMock = new Mock<IBackgroundJobClient>();
    _mapperMock = new Mock<IMapper>();

    _handler = new ScheduleEmailCommandHandler(
        _context,
        _mergerMock.Object,
        _jobClientMock.Object,
        _mapperMock.Object
    );
  }

  [Fact]
  public async Task Handle_ShouldEnqueueJob_WhenSendAtIsNull()
  {
    // Arrange
    var contactId = Guid.NewGuid();
    var templateId = Guid.NewGuid();

    // Note: Using full Entity definition
    var contact = new Contact { Id = contactId, FirstName = "Slater", Email = "slater@crm.com" };
    var template = new Template { Id = templateId, Name = "Welcome", Content = "Hello {{FirstName}}" };

    await _context.Contacts.AddAsync(contact);
    await _context.Templates.AddAsync(template);
    await _context.SaveChangesAsync();

    var command = new ScheduleEmailCommand(contactId, templateId, null);

    _mapperMock.Setup(m => m.Map<ContactDto>(contact)).Returns(new ContactDto());
    _mergerMock.Setup(m => m.Merge(It.IsAny<string>(), It.IsAny<ContactDto>())).Returns("Merged Content");

    // Act
    await _handler.Handle(command, CancellationToken.None);

    // Assert
    // Verify that Enqueue was called specifically for IEmailSender
    _jobClientMock.Verify(x => x.Create(
        It.Is<Job>(job => job.Type == typeof(IEmailSender) && job.Method.Name == "SendEmailAsync"),
        It.IsAny<EnqueuedState>()));
  }

  [Fact]
  public async Task Handle_ShouldScheduleJob_WhenSendAtIsFuture()
  {
    // Arrange
    var contactId = Guid.NewGuid();
    var templateId = Guid.NewGuid();
    var futureDate = DateTime.UtcNow.AddDays(1);

    var contact = new Contact { Id = contactId, FirstName = "Slater", Email = "slater@crm.com" };
    var template = new Template { Id = templateId, Name = "Welcome", Content = "Hello" };

    await _context.Contacts.AddAsync(contact);
    await _context.Templates.AddAsync(template);
    await _context.SaveChangesAsync();

    var command = new ScheduleEmailCommand(contactId, templateId, futureDate);

    _mapperMock.Setup(m => m.Map<ContactDto>(contact)).Returns(new ContactDto());
    _mergerMock.Setup(m => m.Merge(It.IsAny<string>(), It.IsAny<ContactDto>())).Returns("Merged Content");

    // Act
    await _handler.Handle(command, CancellationToken.None);

    // Assert
    // Verify that Create (Schedule) was called with a ScheduledState
    _jobClientMock.Verify(x => x.Create(
        It.Is<Job>(job => job.Type == typeof(IEmailSender) && job.Method.Name == "SendEmailAsync"),
        It.IsAny<ScheduledState>()));
  }

  [Fact]
  public async Task Handle_ShouldThrowNotFound_WhenContactMissing()
  {
    // Arrange
    var command = new ScheduleEmailCommand(Guid.NewGuid(), Guid.NewGuid(), null);

    // Act
    Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

    // Assert
    await act.Should().ThrowAsync<NotFoundException>();
  }

  public void Dispose()
  {
    _context.Dispose();
  }
}