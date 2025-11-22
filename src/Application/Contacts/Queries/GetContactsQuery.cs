using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Contacts.Queries
{
  public record GetContactsQuery : IRequest<List<ContactDto>>
  {

  }
}

