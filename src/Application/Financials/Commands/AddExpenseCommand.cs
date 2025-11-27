using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Core.Enum;
using CRM_Vivid.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Financials.Commands
{
  public record AddExpenseCommand(
      Guid EventId, // We look up Budget via EventId for safety
      string Description,
      decimal Amount,
      DateTime DateIncurred,
      string Category, // "Catering", "Venue", etc.
      Guid? VendorId,
      int? LinkedDocumentId
  ) : IRequest<Guid>;

  public class AddExpenseCommandHandler : IRequestHandler<AddExpenseCommand, Guid>
  {
    private readonly IApplicationDbContext _context;

    public AddExpenseCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<Guid> Handle(AddExpenseCommand request, CancellationToken cancellationToken)
    {
      // 1. Find the Budget for this Event
      var budget = await _context.Budgets
          .FirstOrDefaultAsync(b => b.EventId == request.EventId, cancellationToken);

      if (budget == null)
      {
        // You cannot add an expense if a budget hasn't been initialized (UpsertBudget first)
        throw new NotFoundException("Budget for Event", request.EventId);
      }

      // 2. Create Expense
      var entity = new Expense
      {
        Id = Guid.NewGuid(),
        BudgetId = budget.Id,
        Description = request.Description,
        Amount = request.Amount,
        DateIncurred = request.DateIncurred,
        VendorId = request.VendorId,
        LinkedDocumentId = request.LinkedDocumentId
      };

      // Parse Enum (Case insensitive)
      if (Enum.TryParse<ExpenseCategory>(request.Category, true, out var cat))
      {
        entity.Category = cat;
      }
      else
      {
        entity.Category = ExpenseCategory.General;
      }

      _context.Expenses.Add(entity);
      await _context.SaveChangesAsync(cancellationToken);

      return entity.Id;
    }
  }
}