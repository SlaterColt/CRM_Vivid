using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM_Vivid.Application.Dashboard.Queries;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetDashboardStatsQueryHandler(IApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
  {
    var now = DateTime.UtcNow;

    // 1. Total Contacts
    var totalContacts = await _context.Contacts.CountAsync(cancellationToken);

    // 2. Active Events (Future dates based on StartDateTime)
    var activeEvents = await _context.Events
        .Where(e => e.StartDateTime >= now)
        .CountAsync(cancellationToken);

    // 3. Pending Tasks (Not Completed)
    var pendingTasks = await _context.Tasks
        .Where(t => t.Status != CRM_Vivid.Core.Enum.TaskStatus.Completed)
        .CountAsync(cancellationToken);

    // 4. Recent Emails (Last 24 Hours)
    var recentEmails = await _context.EmailLogs
        .Where(e => e.SentAt >= now.AddHours(-24))
        .CountAsync(cancellationToken);

    // 5. Upcoming Events List (Next 5)
    var upcomingEvents = await _context.Events
        .Where(e => e.StartDateTime >= now)
        .OrderBy(e => e.StartDateTime)
        .Take(5)
        .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellationToken);

    return new DashboardStatsDto
    {
      TotalContacts = totalContacts,
      ActiveEvents = activeEvents,
      PendingTasks = pendingTasks,
      RecentEmails = recentEmails,
      UpcomingEvents = upcomingEvents
    };
  }
}