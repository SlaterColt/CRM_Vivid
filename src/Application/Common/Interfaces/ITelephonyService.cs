namespace CRM_Vivid.Application.Common.Interfaces;

public interface ITelephonyService
{
  /// <summary>
  /// Sends an SMS message to a single recipient.
  /// </summary>
  /// <param name="toPhoneNumber">The recipient's phone number in E.164 format (e.g., +15551234567).</param>
  /// <param name="body">The message content.</param>
  /// <returns>True if the message was successfully queued for sending.</returns>
  Task<bool> SendSmsAsync(string toPhoneNumber, string body);

  /// <summary>
  /// Sends a standardized alert notification (for critical events like contract changes).
  /// </summary>
  /// <param name="toPhoneNumber">The recipient's phone number.</param>
  /// <param name="alertMessage">The content of the critical alert.</param>
  /// <returns>True if the alert was successfully queued.</returns>
  Task<bool> SendCriticalAlertAsync(string toPhoneNumber, string alertMessage);
}