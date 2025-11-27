using AutoMapper;
using CRM_Vivid.Application.Common.Interfaces;
using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Core.Entities;
using FluentValidation;
using MediatR;

namespace CRM_Vivid.Application.Documents.Commands
{
  public class UploadDocumentCommand : IRequest<DocumentDto>
  {
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long Size { get; set; }
    public required Stream FileContent { get; set; }
    public Guid RelatedEntityId { get; set; }
    public required string RelatedEntityType { get; set; }

    // --- NEW ---
    public string Category { get; set; } = "General";
  }

  public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
  {
    public UploadDocumentCommandValidator()
    {
      RuleFor(v => v.FileName).NotEmpty().MaximumLength(255);
      RuleFor(v => v.ContentType).NotEmpty();
      RuleFor(v => v.RelatedEntityId).NotEmpty();
      RuleFor(v => v.RelatedEntityType).NotEmpty().MaximumLength(50);
      RuleFor(v => v.FileContent).NotNull();
      RuleFor(v => v.Size).GreaterThan(0);
      RuleFor(v => v.Category).MaximumLength(50);
    }
  }

  public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, DocumentDto>
  {
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly IMapper _mapper;

    public UploadDocumentCommandHandler(IApplicationDbContext context, IFileStorageService fileStorage, IMapper mapper)
    {
      _context = context;
      _fileStorage = fileStorage;
      _mapper = mapper;
    }

    public async Task<DocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
      // 1. Upload physical file
      var storedFileName = await _fileStorage.UploadAsync(request.FileContent, request.FileName);

      // 2. Create Entity
      var document = new Document
      {
        FileName = request.FileName,
        StoredFileName = storedFileName,
        ContentType = request.ContentType,
        Size = request.Size,
        RelatedEntityId = request.RelatedEntityId,
        RelatedEntityType = request.RelatedEntityType,
        Category = request.Category ?? "General", // Default if null
        UploadedAt = DateTime.UtcNow
      };

      _context.Documents.Add(document);
      await _context.SaveChangesAsync(cancellationToken);

      // 3. Map and return
      var dto = new DocumentDto
      {
        Id = document.Id,
        FileName = document.FileName,
        ContentType = document.ContentType,
        Size = document.Size,
        UploadedAt = document.UploadedAt,
        RelatedEntityId = document.RelatedEntityId,
        RelatedEntityType = document.RelatedEntityType,
        Category = document.Category, // Map the new field
        Url = _fileStorage.GetFileUrl(storedFileName)
      };

      return dto;
    }
  }
}