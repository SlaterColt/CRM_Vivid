namespace CRM_Vivid.Application.Common.Models
{
  public class ContactDto
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public string? Title { get; set; }
    public string? Organization { get; set; }
  }
}