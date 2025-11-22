using CRM_Vivid.Application.Common.Models;
using MediatR;
using System;

namespace CRM_Vivid.Application.Automation.Commands;

public class ScheduleEmailCommand : IRequest<bool>
{
  public ContactDto Contact { get; set; } = new();
  public string TemplateContent { get; set; } = string.Empty;
  public string Subject { get; set; } = string.Empty;
  public DateTime ScheduleTime { get; set; }
}