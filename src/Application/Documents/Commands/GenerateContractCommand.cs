using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Core.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CRM_Vivid.Application.Documents.Commands;

public record GenerateContractCommand : IRequest<DocumentDto>
{
  public Guid EventId { get; init; }
}

public class GenerateContractCommandHandler : IRequestHandler<GenerateContractCommand, DocumentDto>
{
  private readonly IApplicationDbContext _context;
  private readonly IContractGenerator _contractGenerator;
  private readonly IFileStorageService _fileStorage;

  public GenerateContractCommandHandler(
      IApplicationDbContext context,
      IContractGenerator contractGenerator,
      IFileStorageService fileStorage)
  {
    _context = context;
    _contractGenerator = contractGenerator;
    _fileStorage = fileStorage;
  }

  public async Task<DocumentDto> Handle(GenerateContractCommand request, CancellationToken cancellationToken)
  {
    // 1. Generate PDF Bytes
    byte[] pdfBytes = await _contractGenerator.GenerateContractAsync(request.EventId);

    // 2. Define Metadata
    string fileName = $"Contract_VIVID_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
    string contentType = "application/pdf";

    // 3. Save to Disk (Now uses the byte[] overload we added)
    string fileUrl = await _fileStorage.SaveFileAsync(pdfBytes, fileName);

    // 4. Create Entity
    var document = new Document
    {
      FileName = fileName,
      StoredFileName = fileName, // Required property
      ContentType = contentType,
      Size = pdfBytes.Length,
      Url = fileUrl, // Now valid because we added it to Entity
      UploadedAt = DateTime.UtcNow,
      RelatedEntityId = request.EventId,
      RelatedEntityType = "Event",
      Category = "Contract"
    };

    _context.Documents.Add(document);
    await _context.SaveChangesAsync(cancellationToken);

    // 5. Return DTO
    return new DocumentDto
    {
      Id = document.Id,
      FileName = document.FileName,
      ContentType = document.ContentType,
      Size = document.Size,
      Url = document.Url,
      UploadedAt = document.UploadedAt,
      Category = document.Category,
      // FIX: Removed .ToString() because DocumentDto expects a Guid here (CS0029)
      RelatedEntityId = document.RelatedEntityId,
      RelatedEntityType = document.RelatedEntityType
    };
  }
}