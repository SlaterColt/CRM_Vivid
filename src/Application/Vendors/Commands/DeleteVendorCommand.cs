using MediatR;

namespace CRM_Vivid.Application.Vendors.Commands;

public class DeleteVendorCommand : IRequest<bool>
{
  public Guid Id { get; set; }
}