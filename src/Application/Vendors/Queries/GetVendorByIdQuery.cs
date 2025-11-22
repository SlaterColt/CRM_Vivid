using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Vendors.Queries;

public class GetVendorByIdQuery : IRequest<VendorDto>
{
  public Guid Id { get; set; }
}