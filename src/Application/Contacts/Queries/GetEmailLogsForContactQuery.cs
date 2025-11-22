using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM_Vivid.Application.Contacts.Queries;

public record GetEmailLogsForContactQuery(Guid ContactId) : IRequest<List<EmailLogDto>>;

public class GetEmailLogsForContactQueryHandler : IRequestHandler<GetEmailLogsForContactQuery, List<EmailLogDto>>
{
  private readonly IApplicationDbContext _context;

  public GetEmailLogsForContactQueryHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<List<EmailLogDto>> Handle(GetEmailLogsForContactQuery request, CancellationToken cancellationToken)
  {
    // Optional: Validate Contact exists first
    var contactExists = await _context.Contacts
        .AnyAsync(c => c.Id == request.ContactId, cancellationToken);

    if (!contactExists)
    {
      throw new NotFoundException(nameof(Contact), request.ContactId);
    }

    // Fetch Logs
    return await _context.EmailLogs
        .Where(e => e.ContactId == request.ContactId)
        .OrderByDescending(e => e.SentAt)
        .Select(e => new EmailLogDto
        {
          Id = e.Id,
          To = e.To,
          Subject = e.Subject,
          SentAt = e.SentAt,
          IsSuccess = e.IsSuccess,
          ErrorMessage = e.ErrorMessage
        })
        .ToListAsync(cancellationToken);
  }
}