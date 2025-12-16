// FILE: src/Application/Contacts/Queries/GetContactTaskSummaryQuery.cs (NEW FILE)
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Contacts.Queries;

public record GetContactTaskSummaryQuery(Guid ContactId) : IRequest<ContactTaskSummaryDto>;

public class GetContactTaskSummaryQueryHandler : IRequestHandler<GetContactTaskSummaryQuery, ContactTaskSummaryDto>
{
  private readonly IApplicationDbContext _context;

  public GetContactTaskSummaryQueryHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<ContactTaskSummaryDto> Handle(GetContactTaskSummaryQuery request, CancellationToken cancellationToken)
  {
    var now = DateTime.UtcNow;

    var result = await _context.Contacts
        .Where(c => c.Id == request.ContactId)
        .Select(c => new ContactTaskSummaryDto
        {
          ContactId = c.Id,
          ContactName = $"{c.FirstName} {c.LastName}",

          // Calculate 1: Total Tasks Assigned (via Tasks back-reference)
          TotalTasksAssigned = c.Tasks.Count(),

          // Calculate 2: Total Tasks Completed
          TotalTasksCompleted = c.Tasks.Count(t => t.Status == Core.Enum.TaskStatus.Completed),

          // Calculate 3: Total Tasks Overdue 
          // Status is NOT Completed AND DueDate is in the past (before now)
          TotalTasksOverdue = c.Tasks.Count(t =>
                  t.Status != Core.Enum.TaskStatus.Completed &&
                  t.DueDate.HasValue &&
                  t.DueDate.Value < now)
        })
        .SingleOrDefaultAsync(cancellationToken);

    if (result == null)
    {
      throw new NotFoundException(nameof(Contact), request.ContactId);
    }

    return result;
  }
}