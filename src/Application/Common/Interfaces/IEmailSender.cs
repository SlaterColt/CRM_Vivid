namespace CRM_Vivid.Application.Common.Interfaces;

public interface IEmailSender
{
  Task SendEmailAsync(
      string to,
      string subject,
      string body,
      Guid? templateId = null, // NEW
      Guid? eventId = null);   // NEW
}
