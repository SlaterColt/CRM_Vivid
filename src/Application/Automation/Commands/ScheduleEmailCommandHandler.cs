using AutoMapper;
using CRM_Vivid.Application.Common.Interfaces; // Correct Interface location
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Exceptions;
// using CRM_Vivid.Application.Common.Interfaces; <-- REMOVED (Source of Error)
using CRM_Vivid.Core.Entities;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace CRM_Vivid.Application.Automation.Commands;

public class ScheduleEmailCommandHandler : IRequestHandler<ScheduleEmailCommand, Unit>
{
  private readonly IApplicationDbContext _context;
  private readonly ITemplateMerger _merger;
  private readonly IBackgroundJobClient _jobClient;
  private readonly IMapper _mapper;

  public ScheduleEmailCommandHandler(
      IApplicationDbContext context,
      ITemplateMerger merger,
      IBackgroundJobClient jobClient,
      IMapper mapper)
  {
    _context = context;
    _merger = merger;
    _jobClient = jobClient;
    _mapper = mapper;
  }

  public async Task<Unit> Handle(ScheduleEmailCommand request, CancellationToken cancellationToken)
  {
    // 1. Fetch the Contact
    var contact = await _context.Contacts
        .FirstOrDefaultAsync(c => c.Id == request.ContactId, cancellationToken);

    if (contact == null)
    {
      throw new NotFoundException(nameof(Contact), request.ContactId);
    }

    // 2. Fetch the Template
    var template = await _context.Templates
        .FirstOrDefaultAsync(t => t.Id == request.TemplateId, cancellationToken);

    if (template == null)
    {
      throw new NotFoundException(nameof(Template), request.TemplateId);
    }

    // 3. Map & Merge
    var contactDto = _mapper.Map<ContactDto>(contact);
    var mergedBody = _merger.Merge(template.Content, contactDto);
    var subject = template.Name;

    // 4. Schedule via Hangfire
    // Note: We pass the ContactId to the sender in the next step so we can log it!
    if (request.SendAt.HasValue && request.SendAt.Value > DateTime.UtcNow)
    {
      _jobClient.Schedule<IEmailSender>(
          sender => sender.SendEmailAsync(contact.Email, subject, mergedBody),
          request.SendAt.Value
      );
    }
    else
    {
      _jobClient.Enqueue<IEmailSender>(
          sender => sender.SendEmailAsync(contact.Email, subject, mergedBody)
      );
    }

    return Unit.Value;
  }
}