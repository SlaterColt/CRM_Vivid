using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Financials.Commands;
using CRM_Vivid.Application.Financials.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CRM_Vivid.Api.Controllers
{
  [ApiController]
  [Route("api/events/{eventId}/financials")] // Sub-resource routing
  public class FinancialsController : ControllerBase
  {
    private readonly MediatR.ISender _mediator;

    public FinancialsController(MediatR.ISender mediator)
    {
      _mediator = mediator;
    }

    // GET: api/events/{id}/financials
    [HttpGet]
    public async Task<ActionResult<EventFinancialsDto>> Get(Guid eventId)
    {
      return await _mediator.Send(new GetEventFinancialsQuery(eventId));
    }

    // POST: api/events/{id}/financials/budget
    // Initialize or Update the Total Budget
    [HttpPost("budget")]
    public async Task<ActionResult<Guid>> UpsertBudget(Guid eventId, [FromBody] UpsertBudgetCommand command)
    {
      if (eventId != command.EventId) return BadRequest();
      return await _mediator.Send(command);
    }

    // POST: api/events/{id}/financials/expenses
    // Add a line item
    [HttpPost("expenses")]
    public async Task<ActionResult<Guid>> AddExpense(Guid eventId, [FromBody] AddExpenseCommand command)
    {
      if (eventId != command.EventId) return BadRequest();
      return await _mediator.Send(command);
    }
  }
}