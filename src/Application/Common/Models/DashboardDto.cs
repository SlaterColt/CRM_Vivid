using System.Collections.Generic;

namespace CRM_Vivid.Application.Common.Models;

public class DashboardStatsDto
{
  public int TotalContacts { get; set; }
  public int ActiveEvents { get; set; }
  public int PendingTasks { get; set; }
  public int RecentEmails { get; set; }
  public List<EventDto> UpcomingEvents { get; set; } = new();
}