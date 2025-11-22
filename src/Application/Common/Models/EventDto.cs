namespace CRM_Vivid.Application.Common.Models
{
  public class EventDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? Location { get; set; }
    public bool IsPublic { get; set; }
    public string Status { get; set; } = string.Empty; // AutoMapper will map the Enum to a string
  }
}