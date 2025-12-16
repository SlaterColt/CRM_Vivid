namespace CRM_Vivid.Core.Entities
{
  public class EmailLog
  {
    public Guid Id { get; set; }
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    // Foreign Keys
    public Guid? ContactId { get; set; }
    public virtual Contact? Contact { get; set; }

    public Guid? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public Guid? EventId { get; set; }
    public Event? Event { get; set; }

    public Guid? TemplateId { get; set; }
    public Template? Template { get; set; }
  }
}