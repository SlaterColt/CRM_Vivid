namespace CRM_Vivid.Application.Common.Models
{
  public class EmailLogDto
  {
    public Guid Id { get; set; }
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
  }
}