using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CRM_Vivid.Application.Common.Interfaces;

public interface IBackgroundJobService
{
  /// <summary>
  /// Enqueues a job to execute once at a specified future time.
  /// </summary>
  /// <param name="methodCall">The method to call (e.g., () => IEmailSender.SendEmailAsync(...))</param>
  /// <param name="enqueueAt">The date and time to execute the job.</param>
  /// <returns>The Hangfire Job ID.</returns>
  string Schedule(Expression<System.Action> methodCall, DateTimeOffset enqueueAt);

  /// <summary>
  /// Enqueues a job for immediate, reliable execution.
  /// </summary>
  /// <param name="methodCall">The method to call.</param>
  /// <returns>The Hangfire Job ID.</returns>
  string Enqueue(Expression<System.Action> methodCall);
}