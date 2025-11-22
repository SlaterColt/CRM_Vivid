using CRM_Vivid.Application.Common.Models;
using MediatR;

namespace CRM_Vivid.Application.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;