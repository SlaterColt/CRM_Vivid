using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CRM_Vivid.Core.Entities; // Required for AnyAsync

namespace CRM_Vivid.Application.Documents.Commands;

// Command returns the raw PDF bytes
public record GenerateContractCommand : IRequest<byte[]>
{
  public Guid EventId { get; init; }
}

public class GenerateContractCommandValidator : AbstractValidator<GenerateContractCommand>
{
  public GenerateContractCommandValidator()
  {
    RuleFor(v => v.EventId)
        .NotEmpty().WithMessage("Event ID is required to generate a contract.");
  }
}

public class GenerateContractCommandHandler : IRequestHandler<GenerateContractCommand, byte[]>
{
  private readonly IContractGenerator _contractGenerator;
  private readonly IApplicationDbContext _context;

  public GenerateContractCommandHandler(IContractGenerator contractGenerator, IApplicationDbContext context)
  {
    _contractGenerator = contractGenerator;
    _context = context;
  }

  public async Task<byte[]> Handle(GenerateContractCommand request, CancellationToken cancellationToken)
  {
    // Sanity check: Ensure the event exists before generating a contract for it.
    var eventExists = await _context.Events.AnyAsync(e => e.Id == request.EventId, cancellationToken);

    if (!eventExists)
    {
      throw new NotFoundException(nameof(Event), request.EventId);
    }

    // Call the service to create the document
    var pdfBytes = await _contractGenerator.GenerateContractAsync(request.EventId);

    return pdfBytes;
  }
}