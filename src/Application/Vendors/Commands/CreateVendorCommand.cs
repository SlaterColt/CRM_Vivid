// FILE: src/Application/Vendors/Commands/CreateVendorCommand.cs
using MediatR;

namespace CRM_Vivid.Application.Vendors.Commands;

public class CreateVendorCommand : IRequest<Guid>
{
  public string Name { get; set; } = string.Empty;
  public string? PhoneNumber
  {
    get;
    set;
  }
  public string? Email { get; set; }
  public string ServiceType { get; set; } = string.Empty;
  // <-- FIX: Changed from VendorType to string

  // NEW: Flexible Attributes as a JSON string
  public string? Attributes { get; set; }
}