using MediatR;

namespace CRM_Vivid.Application.Contacts.Commands;

// This record just needs to hold the ID of the contact to delete.
// It also returns 'Unit' (nothing) on success.
public record DeleteContactCommand(Guid Id) : IRequest<Unit>;