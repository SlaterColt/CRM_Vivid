// FILE: src/Application/Common/Models/VendorDto.cs
namespace CRM_Vivid.Application.Common.Models;

public class VendorDto
{
  public Guid Id { get; set; }
  public string Name
  {
    get; set;
  } = string.Empty;
  public string? PhoneNumber { get; set; }
  public string? Email
  {
    get; set;
  }
  public string ServiceType { get; set; } = string.Empty;
  public string? Attributes { get; set; }
  public string? Role { get; set; }
}