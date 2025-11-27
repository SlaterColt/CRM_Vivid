using CRM_Vivid.Application.Common.Interfaces;
using Hangfire;
using System.Linq.Expressions;

namespace CRM_Vivid.Infrastructure.Services;

public class HangfireJobService : IBackgroundJobService
{
  public string Schedule(Expression<System.Action> methodCall, DateTimeOffset enqueueAt)
  {
    // Hangfire implementation for delayed job
    return BackgroundJob.Schedule(methodCall, enqueueAt);
  }

  public string Enqueue(Expression<System.Action> methodCall)
  {
    // Hangfire implementation for immediate, reliable job
    return BackgroundJob.Enqueue(methodCall);
  }
}