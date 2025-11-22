using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Vendors.Queries;

public class GetVendorsQuery : IRequest<IEnumerable<VendorDto>>
{
}