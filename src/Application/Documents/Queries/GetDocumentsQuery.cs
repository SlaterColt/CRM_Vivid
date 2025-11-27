using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM_Vivid.Application.Documents.Queries;

public class GetDocumentsQuery : IRequest<List<DocumentDto>>
{
  public Guid RelatedEntityId { get; set; }
  public required string RelatedEntityType { get; set; }
}

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, List<DocumentDto>>
{
  private readonly IApplicationDbContext _context;
  private readonly IFileStorageService _fileStorage;

  public GetDocumentsQueryHandler(IApplicationDbContext context, IFileStorageService fileStorage)
  {
    _context = context;
    _fileStorage = fileStorage;
  }

  public async Task<List<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
  {
    var documents = await _context.Documents
        .Where(d => d.RelatedEntityId == request.RelatedEntityId && d.RelatedEntityType == request.RelatedEntityType)
        .OrderByDescending(d => d.UploadedAt)
        .ToListAsync(cancellationToken);

    var dtos = documents.Select(d => new DocumentDto
    {
      Id = d.Id,
      FileName = d.FileName,
      ContentType = d.ContentType,
      Size = d.Size,
      UploadedAt = d.UploadedAt,
      RelatedEntityId = d.RelatedEntityId,
      RelatedEntityType = d.RelatedEntityType,
      Category = d.Category, // Map the Category
      Url = _fileStorage.GetFileUrl(d.StoredFileName)
    }).ToList();

    return dtos;
  }
}