using AutoMapper;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM_Vivid.Application.Financials.Queries
{
  public class GetEventFinancialsQueryHandler : IRequestHandler<GetEventFinancialsQuery, EventFinancialsDto>
  {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEventFinancialsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<EventFinancialsDto> Handle(GetEventFinancialsQuery request, CancellationToken cancellationToken)
    {
      // 1. Fetch Event with Budget and deep relations (Expenses -> Vendor/Document)
      var eventEntity = await _context.Events
          .Include(e => e.Budget)
          .ThenInclude(b => b!.Expenses)
          .ThenInclude(ex => ex.Vendor)
          .Include(e => e.Budget)
          .ThenInclude(b => b!.Expenses)
          .ThenInclude(ex => ex.LinkedDocument)
          .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

      if (eventEntity == null)
      {
        throw new NotFoundException(nameof(Event), request.EventId);
      }

      // 2. Initialize DTO
      var dto = new EventFinancialsDto
      {
        EventId = eventEntity.Id,
        EventName = eventEntity.Name,
        Currency = eventEntity.Budget?.Currency ?? "USD",
        BudgetTotal = eventEntity.Budget?.TotalAmount ?? 0,
        Notes = eventEntity.Budget?.Notes
      };

      // 3. Process Expenses if Budget exists
      if (eventEntity.Budget != null)
      {
        dto.Expenses = _mapper.Map<List<ExpenseDto>>(eventEntity.Budget.Expenses);

        // The Calculator Logic
        dto.TotalSpent = dto.Expenses.Sum(e => e.Amount);
      }

      return dto;
    }
  }
}