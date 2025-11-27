using CRM_Vivid.Application.Common.Interfaces;
using MediatR;

namespace CRM_Vivid.Application.Automation.Commands;

public class ScheduleEmailCommandHandler : IRequestHandler<ScheduleEmailCommand, bool>
{
  private readonly ITemplateMerger _merger;
  private readonly IEmailSender _emailSender;
  private readonly IApplicationDbContext _context;

  public ScheduleEmailCommandHandler(
      ITemplateMerger merger,
      IEmailSender emailSender,
      IApplicationDbContext context)
  {
    _merger = merger;
    _emailSender = emailSender;
    _context = context;
  }

  public async Task<bool> Handle(ScheduleEmailCommand request, CancellationToken cancellationToken)
  {
    // NOTE: This is a legacy handler. Ideally, we fetch data here. 
    // For now, we assume the request has what we need or we construct a basic dictionary.

    // Create the dictionary for the merger
    var placeholders = new Dictionary<string, string>
        {
            { "FirstName", request.Contact.FirstName ?? "" },
            { "LastName", request.Contact.LastName ?? "" },
            { "Organization", request.Contact.Organization ?? "" },
            { "Title", request.Contact.Title ?? "" },
            { "ContactName", $"{request.Contact.FirstName} {request.Contact.LastName}" }
        };

    var mergedBody = _merger.Merge(request.TemplateContent, placeholders);

    await _emailSender.SendEmailAsync(request.Contact.Email, request.Subject, mergedBody);

    return true;
  }
}