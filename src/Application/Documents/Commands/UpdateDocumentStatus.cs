using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using CRM_Vivid.Core.Entities;
using CRM_Vivid.Core.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TTask = System.Threading.Tasks.Task;

namespace CRM_Vivid.Application.Documents.Commands
{
  public record UpdateDocumentStatusCommand(
      int DocumentId,
      ContractStatus NewStatus) : IRequest;

  public class UpdateDocumentStatusCommandHandler : IRequestHandler<UpdateDocumentStatusCommand>
  {
    private readonly IApplicationDbContext _context;

    public UpdateDocumentStatusCommandHandler(IApplicationDbContext context)
    {
      _context = context;
    }

    public async TTask Handle(UpdateDocumentStatusCommand request, CancellationToken cancellationToken)
    {
      // 1. Find the Document (must be tracked for SaveChanges)
      var document = await _context.Documents
          .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

      if (document == null)
      {
        throw new NotFoundException(nameof(Document), request.DocumentId.ToString());
      }

      document.Status = request.NewStatus;

      if (request.NewStatus == ContractStatus.Signed)
      {
        document.SignedAt = DateTime.UtcNow;

        // --- INTERNAL COMPLETION LOGIC TRIGGER: LOCK BUDGET (STRING COMPARISON FIX) ---

        // Load the Budget entity with tracking. We are enforcing the Guid comparison 
        // by converting both sides to string to overcome subtle EF tracking/comparison issues.
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(
                b => b.EventId.ToString() == document.RelatedEntityId.ToString(),
                cancellationToken);

        if (budget != null)
        {
          if (!budget.IsLocked)
          {
            // Update tracked entity directly
            budget.IsLocked = true;
          }
        }
        // If the budget doesn't exist, we silently proceed (Document status still updated).
      }

      // Save changes for both the Document (Status) and the Budget (IsLocked)
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}