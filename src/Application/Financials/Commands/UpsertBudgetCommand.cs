using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM_Vivid.Application.Financials.Commands
{
  public record UpsertBudgetCommand(Guid EventId, decimal TotalAmount, string Currency, string? Notes) : IRequest<Guid>;

  public class UpsertBudgetCommandHandler : IRequestHandler<UpsertBudgetCommand, Guid>
  {
    private readonly IApplicationDbContext _context;

    public UpsertBudgetCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Guid> Handle(UpsertBudgetCommand request, CancellationToken cancellationToken)
    {
      var eventEntity = await _context.Events
          .Include(e => e.Budget)
          .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

      if (eventEntity == null)
      {
        throw new NotFoundException(nameof(Event), request.EventId);
      }

      if (eventEntity.Budget == null)
      {
        // Create
        var budget = new Budget
        {
          Id = Guid.NewGuid(),
          EventId = request.EventId,
          TotalAmount = request.TotalAmount,
          Currency = request.Currency,
          Notes = request.Notes
        };
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync(cancellationToken);
        return budget.Id;
      }
      else
      {
        // Update
        eventEntity.Budget.TotalAmount = request.TotalAmount;
        eventEntity.Budget.Currency = request.Currency;
        eventEntity.Budget.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
        return eventEntity.Budget.Id;
      }
    }
  }
}