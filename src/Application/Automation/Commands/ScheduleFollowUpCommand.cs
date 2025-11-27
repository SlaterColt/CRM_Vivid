using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Enum;
using MediatR;
using FluentValidation;
using System.Linq.Expressions;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Entities; // <-- FIX 1: ADDED MISSING ENTITY REFERENCE

namespace CRM_Vivid.Application.Automation.Commands;

// This command is the main request object
public record ScheduleFollowUpCommand : IRequest<string>
{
  public Guid ContactId { get; init; }
  public Guid? EventId { get; init; }
  public Guid TemplateId { get; init; }

  public DateTime ScheduleTime { get; init; }
  public CommunicationType Type { get; init; } // Email or SMS (future-proofing)
}

public class ScheduleFollowUpCommandValidator : AbstractValidator<ScheduleFollowUpCommand>
{
  public ScheduleFollowUpCommandValidator()
  {
    RuleFor(v => v.ContactId).NotEmpty().WithMessage("Contact ID is required for follow-up scheduling.");
    RuleFor(v => v.TemplateId).NotEmpty().WithMessage("Template ID is required.");
    RuleFor(v => v.ScheduleTime).NotEmpty().GreaterThan(DateTime.UtcNow).WithMessage("Schedule time must be in the future.");
    RuleFor(v => v.Type).IsInEnum().WithMessage("Invalid communication type specified.");
  }
}

public class ScheduleFollowUpCommandHandler : IRequestHandler<ScheduleFollowUpCommand, string>
{
  private readonly IBackgroundJobService _jobService;
  private readonly IApplicationDbContext _context;
  private readonly ITemplateMerger _merger;
  private readonly IEmailSender _emailSender;
  // private readonly ITelephonyService _smsSender; // Deferred until multi-tenancy is solved

  public ScheduleFollowUpCommandHandler(
      IBackgroundJobService jobService,
      IApplicationDbContext context,
      ITemplateMerger merger,
      IEmailSender emailSender)
  {
    _jobService = jobService;
    _context = context;
    _merger = merger;
    _emailSender = emailSender;
  }

  public async Task<string> Handle(ScheduleFollowUpCommand request, CancellationToken cancellationToken)
  {
    // 1. Fetch Contact and Template (Necessary for Merge Context)
    var contact = await _context.Contacts.FindAsync(new object[] { request.ContactId }, cancellationToken);
    var template = await _context.Templates.FindAsync(new object[] { request.TemplateId }, cancellationToken);

    if (contact == null) throw new NotFoundException(nameof(Contact), request.ContactId);
    if (template == null) throw new NotFoundException(nameof(template), request.TemplateId);
    if (string.IsNullOrWhiteSpace(contact.Email)) throw new Exception($"Contact {contact.FirstName} has no email address for scheduling.");

    // 2. Prepare Placeholder Dictionary (Simplified for now)
    var placeholders = new Dictionary<string, string>
        {
            { "FirstName", contact.FirstName ?? "" },
            { "EventId", request.EventId?.ToString() ?? "N/A" }
        };
    var mergedBody = _merger.Merge(template.Content ?? "", placeholders);
    var mergedSubject = _merger.Merge(template.Subject ?? "", placeholders);

    // 3. Define the Job Call (The job that Hangfire will execute)
    Expression<Action> jobCall = () => _emailSender.SendEmailAsync(
        contact.Email,
        mergedSubject,
        mergedBody);

    // 4. Schedule the Job using IBackgroundJobService (Hangfire)
    var jobId = _jobService.Schedule(jobCall, request.ScheduleTime);

    // 5. Optionally, log the schedule in a new EmailLog entry, referencing the jobId.

    return jobId;
  }
}