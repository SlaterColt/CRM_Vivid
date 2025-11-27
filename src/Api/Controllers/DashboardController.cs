using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Application.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Vivid.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
  private readonly IMediator _mediator;

  public DashboardController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet]
  public async Task<ActionResult<DashboardStatsDto>> GetStats()
  {
    return await _mediator.Send(new GetDashboardStatsQuery());
  }
}