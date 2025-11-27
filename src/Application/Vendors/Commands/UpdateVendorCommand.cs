using MediatR;

namespace CRM_Vivid.Application.Vendors.Commands;

public class UpdateVendorCommand : IRequest<bool>
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; }
  public string? Email { get; set; }
  public string ServiceType { get; set; } = string.Empty;// <-- FIX: Changed from VendorType to string
}