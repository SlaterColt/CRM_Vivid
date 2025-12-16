// FILE: src/Application/Automation/Commands/SendSmsCommand.cs (NEW FILE)
using MediatR;
using FluentValidation;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using CRM_Vivid.Core.Entities; // For Contact

namespace CRM_Vivid.Application.Automation.Commands;

public record SendSmsCommand : IRequest<bool>
{
  public Guid ContactId { get; init; }
  public Guid TemplateId { get; init; }
  // Optional: Allows linking SMS to an ongoing event
  public Guid? EventId { get; init; }
}

public class SendSmsCommandValidator : AbstractValidator<SendSmsCommand>
{
  public SendSmsCommandValidator()
  {
    RuleFor(v => v.ContactId).NotEmpty();
    RuleFor(v => v.TemplateId).NotEmpty();
  }
}

public class SendSmsCommandHandler : IRequestHandler<SendSmsCommand, bool>
{
  private readonly IApplicationDbContext _context;
  private readonly ITemplateMerger _merger;
  private readonly ITelephonyService _smsSender;

  public SendSmsCommandHandler(
      IApplicationDbContext context,
      ITemplateMerger merger,
      ITelephonyService smsSender)
  {
    _context = context;
    _merger = merger;
    _smsSender = smsSender;
  }

  public async Task<bool> Handle(SendSmsCommand request, CancellationToken cancellationToken)
  {
    // 1. Fetch Contact, ensuring phone number exists
    var contact = await _context.Contacts.FindAsync(new object[] { request.ContactId }, cancellationToken);
    if (contact == null) throw new NotFoundException(nameof(Contact), request.ContactId);

    // CRITICAL: We need a phone number
    if (string.IsNullOrWhiteSpace(contact.PhoneNumber))
      throw new ArgumentException($"Contact {contact.FirstName} has no phone number for SMS.");

    // 2. Fetch Template
    var template = await _context.Templates.FindAsync(new object[] { request.TemplateId }, cancellationToken);
    if (template == null) throw new NotFoundException(nameof(Template), request.TemplateId);

    // Check if template type is SMS (optional check, but good practice)
    if (template.Type != Core.Enum.TemplateType.SMS)
      throw new ArgumentException($"Template ID {request.TemplateId} is not an SMS template.");


    // 3. Prepare Placeholder Dictionary (Simplified for now)
    var placeholders = new Dictionary<string, string>
    {
        { "FirstName", contact.FirstName ?? "" },
        { "EventId", request.EventId?.ToString() ?? "N/A" }
        // Add more placeholders as needed (e.g., Event Name, Date)
    };

    // 4. Merge Content (Template content is usually short text for SMS)
    var mergedBody = _merger.Merge(template.Content ?? "", placeholders);

    // 5. Send SMS
    var success = await _smsSender.SendSmsAsync(
        toPhoneNumber: contact.PhoneNumber,
        body: mergedBody
    );

    // Note: We would ideally log this SMS send in a new SmsLog entity (future phase).

    return success;
  }
}