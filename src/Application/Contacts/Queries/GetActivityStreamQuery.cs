// FILE: src/Application/Contacts/Queries/GetActivityStreamQuery.cs (FINAL CORRECTED VERSION)
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic; // Required for List

namespace CRM_Vivid.Application.Contacts.Queries;

public record GetActivityStreamQuery(Guid ContactId) : IRequest<List<ActivityDto>>;

public class GetActivityStreamQueryHandler : IRequestHandler<GetActivityStreamQuery, List<ActivityDto>>
{
  private readonly IApplicationDbContext _context;

  public GetActivityStreamQueryHandler(IApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<List<ActivityDto>> Handle(GetActivityStreamQuery request, CancellationToken cancellationToken)
  {
    var contactId = request.ContactId;

    // --- 1. Map Tasks and execute immediately ---
    var taskActivities = await _context.Tasks
        .Where(t => t.ContactId == contactId)
        .Select(t => new ActivityDto
        {
          Id = t.Id,
          Timestamp = t.CreatedAt,
          ActivityType = "TASK",
          Title = t.Title,
          Content = t.Description,
          Status = t.Status.ToString(),
          RelatedEntityId = t.EventId
        })
        .ToListAsync(cancellationToken); // Execute query 1

    // --- 2. Map Notes and execute immediately ---
    var noteActivities = await _context.Notes
        .Where(n => n.ContactId == contactId)
        .Select(n => new ActivityDto
        {
          Id = n.Id,
          Timestamp = n.CreatedAt,
          ActivityType = "NOTE",
          Title = n.Title,
          Content = n.Content,
          Status = "Logged",
          RelatedEntityId = n.EventId
        })
        .ToListAsync(cancellationToken); // Execute query 2

    // --- 3. Map EmailLogs and execute immediately ---
    var emailActivities = await _context.EmailLogs
        .Where(e => e.ContactId == contactId)
        .Select(e => new ActivityDto
        {
          Id = e.Id,
          Timestamp = e.SentAt,
          ActivityType = "EMAIL",
          Title = e.Subject,
          Content = e.Body != null ? (e.Body.Length > 200 ? e.Body.Substring(0, 200) + "..." : e.Body) : null,
          Status = e.IsSuccess ? "Sent Success" : "Sent Failed",
          RelatedEntityId = e.EventId ?? e.TemplateId
        })
        .ToListAsync(cancellationToken); // Execute query 3

    // --- 4. Merge (In Memory) and Order ---
    var activityStream = taskActivities
        .Concat(noteActivities) // Concatenate in memory (C# List<T> operation)
        .Concat(emailActivities) // No more complex LINQ .Union() to break the provider
        .OrderByDescending(a => a.Timestamp)
        .ToList();

    return activityStream;
  }
}