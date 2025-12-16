// FILE: src/Application/Contacts/Commands/InitializeTestFixturesCommand.cs (FINAL CORRECTED VERSION)
using MediatR;
using System;
using CRM_Vivid.Core.Enum;
using CRM_Vivid.Application.Features.Contacts.Commands;
using CRM_Vivid.Application.Common.Interfaces; // Required for User Service/Scoping

namespace CRM_Vivid.Application.Contacts.Commands;

public record InitializeTestFixturesCommand : IRequest<Guid>;

public class InitializeTestFixturesCommandHandler : IRequestHandler<InitializeTestFixturesCommand, Guid>
{
  private readonly ISender _mediator;
  private readonly ICurrentUserService _currentUserService;

  // We inject ISender (Mediator) here so we can chain other commands
  public InitializeTestFixturesCommandHandler(ISender mediator, ICurrentUserService currentUserService)
  {
    _mediator = mediator;
    _currentUserService = currentUserService;
  }

  public async Task<Guid> Handle(InitializeTestFixturesCommand request, CancellationToken cancellationToken)
  {
    // The individual commands (CreateVendorCommand, CreateEventCommand, etc.) 
    // are responsible for setting the CreatedByUserId via the current scope.

    var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
    // 1. Create a Primary Vendor (ENSURE VALIDATION PASSES)
    var vendorId = await _mediator.Send(new Vendors.Commands.CreateVendorCommand
    {
      Name = "The Test Caterer",
      Email = $"test.vendor.{uniqueSuffix}@vivid.com",
      ServiceType = VendorType.Catering.ToString(),
      // FIX: Correct JSON string for Attributes field.
      Attributes = "{}"
    });

    // 2. Create a Primary Contact 
    var contactId = await _mediator.Send(new CreateContactCommand
    {
      FirstName = "Test",
      LastName = "Client",
      Email = $"test.client.{uniqueSuffix}@vivid.com",
      Stage = LeadStage.InDiscussion
    });

    // 3. Create a Primary Event (ENSURE VALIDATION PASSES)
    var eventId = await _mediator.Send(new Events.Commands.CreateEventCommand
    {
      Name = "Fixture Test Event",
      StartDateTime = DateTime.UtcNow.AddDays(30),
      EndDateTime = DateTime.UtcNow.AddDays(30).AddHours(4),
      IsPublic = false,
      // FIX: Ensure Location is set to pass required validation.
      Location = "Virtual Test Hall",
      Description = "Standard fixture event for audit."
    });

    // 4. Link Contact to Event 
    await _mediator.Send(new Events.Commands.AddContactToEventCommand
    {
      EventId = eventId,
      ContactId = contactId,
      Role = "Client Lead"
    });

    // 5. Link Vendor to Event 
    await _mediator.Send(new Events.Commands.AddVendorToEventCommand
    {
      EventId = eventId,
      VendorId = vendorId,
      Role = "Primary Catering"
    });

    return contactId;
  }
}