using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Exceptions; // Corrected Namespace
using CRM_Vivid.Core.Entities;
using MediatR;

// Protocol 10: The Task Alias (Resolves Ambiguity with Core.Entities.Task)
using Task = System.Threading.Tasks.Task;

namespace CRM_Vivid.Application.Documents.Commands;

public record DeleteDocumentCommand(int Id) : IRequest;

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand>
{
  private readonly IApplicationDbContext _context;
  private readonly IFileStorageService _fileStorage;

  public DeleteDocumentCommandHandler(IApplicationDbContext context, IFileStorageService fileStorage)
  {
    _context = context;
    _fileStorage = fileStorage;
  }

  public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
  {
    var entity = await _context.Documents.FindAsync(new object[] { request.Id }, cancellationToken);

    if (entity == null)
    {
      throw new NotFoundException(nameof(Document), request.Id);
    }

    // 1. Delete from Disk (The Shredder)
    await _fileStorage.DeleteAsync(entity.StoredFileName);

    // 2. Delete from Database
    _context.Documents.Remove(entity);

    await _context.SaveChangesAsync(cancellationToken);
  }
}