using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions; // Fixed Namespace
using CRM_Vivid.Core.Enum;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using System;

namespace CRM_Vivid.Application.Templates.Commands;

public enum RecipientType
{
  Contact,
  Vendor
}

public record SendTemplateEmailCommand : IRequest<bool>
{
  public Guid EventId { get; init; }
  public Guid TemplateId { get; init; }
  public Guid TargetEntityId { get; init; }
  public RecipientType RecipientType { get; init; }
}

public class SendTemplateEmailCommandValidator : AbstractValidator<SendTemplateEmailCommand>
{
  public SendTemplateEmailCommandValidator()
  {
    RuleFor(v => v.EventId).NotEmpty();
    RuleFor(v => v.TemplateId).NotEmpty();
    RuleFor(v => v.TargetEntityId).NotEmpty();
  }
}

public class SendTemplateEmailCommandHandler : IRequestHandler<SendTemplateEmailCommand, bool>
{
  private readonly IApplicationDbContext _context;
  private readonly ITemplateMerger _merger;
  private readonly IEmailSender _emailSender;

  public SendTemplateEmailCommandHandler(
      IApplicationDbContext context,
      ITemplateMerger merger,
      IEmailSender emailSender)
  {
    _context = context;
    _merger = merger;
    _emailSender = emailSender;
  }

  public async Task<bool> Handle(SendTemplateEmailCommand request, CancellationToken cancellationToken)
  {
    // 1. Fetch Event
    var evt = await _context.Events
        .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

    if (evt == null) throw new NotFoundException("Event", request.EventId);

    // 2. Fetch Template
    var template = await _context.Templates
        .FindAsync(new object[] { request.TemplateId }, cancellationToken);

    if (template == null) throw new NotFoundException("Template", request.TemplateId);

    // 3. Resolve Recipient & Build Dictionary
    string targetEmail = string.Empty;
    string targetName = string.Empty;

    // Map Event Data
    var placeholders = new Dictionary<string, string>
        {
            { "EventName", evt.Name },
            { "EventDescription", evt.Description ?? "" },
            { "EventDate", evt.StartDateTime.ToShortDateString() }, // Fixed Property
            { "EventLocation", evt.Location ?? "" }
        };

    if (request.RecipientType == RecipientType.Vendor)
    {
      var vendor = await _context.Vendors.FindAsync(new object[] { request.TargetEntityId }, cancellationToken);
      if (vendor == null) throw new NotFoundException("Vendor", request.TargetEntityId);

      targetEmail = vendor.Email ?? string.Empty;
      targetName = vendor.Name;

      placeholders.Add("VendorName", vendor.Name);
      placeholders.Add("ContactName", vendor.Name);
      placeholders.Add("VendorType", vendor.ServiceType.ToString()); // Fixed Property
    }
    else if (request.RecipientType == RecipientType.Contact)
    {
      var contact = await _context.Contacts.FindAsync(new object[] { request.TargetEntityId }, cancellationToken);
      if (contact == null) throw new NotFoundException("Contact", request.TargetEntityId);

      targetEmail = contact.Email ?? string.Empty;
      targetName = $"{contact.FirstName} {contact.LastName}";

      placeholders.Add("ContactName", targetName);
      placeholders.Add("FirstName", contact.FirstName ?? "");
      placeholders.Add("LastName", contact.LastName ?? "");
      placeholders.Add("Organization", contact.Organization ?? "");
    }

    if (string.IsNullOrEmpty(targetEmail))
    {
      // Log or throw? For now, throw to alert the user.
      throw new Exception($"Recipient {targetName} has no email address.");
    }

    // 4. Merge Content
    // Fixed Property: template.Content instead of template.Body
    var mergedSubject = _merger.Merge(template.Subject ?? "No Subject", placeholders);
    var mergedBody = _merger.Merge(template.Content ?? "", placeholders);

    // 5. Send
    await _emailSender.SendEmailAsync(targetEmail, mergedSubject, mergedBody);

    return true;
  }
}