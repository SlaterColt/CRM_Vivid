using CRM_Vivid.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace CRM_Vivid.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
  private readonly IWebHostEnvironment _environment;
  private readonly ILogger<LocalFileStorageService> _logger;
  private const string UploadsFolderName = "uploads";

  public LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger)
  {
    _environment = environment;
    _logger = logger;
  }

  // --- CORE IMPLEMENTATION: STREAM ---
  public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
  {
    var uploadPath = Path.Combine(_environment.WebRootPath, UploadsFolderName);
    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

    var filePath = Path.Combine(uploadPath, fileName);
    using var targetStream = new FileStream(filePath, FileMode.Create);
    await fileStream.CopyToAsync(targetStream);

    return GetFileUrl(fileName);
  }

  // --- INTERFACE ALIAS: UPLOAD (Calls SaveFileAsync) ---
  public Task<string> UploadAsync(Stream fileStream, string fileName)
  {
    return SaveFileAsync(fileStream, fileName);
  }

  // --- CORE IMPLEMENTATION: BYTES (For PDF Generation) ---
  public async Task<string> SaveFileAsync(byte[] fileBytes, string fileName)
  {
    var uploadPath = Path.Combine(_environment.WebRootPath, UploadsFolderName);
    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

    var filePath = Path.Combine(uploadPath, fileName);
    await File.WriteAllBytesAsync(filePath, fileBytes);

    return GetFileUrl(fileName);
  }

  // --- CORE IMPLEMENTATION: DELETE ---
  public Task DeleteFileAsync(string fileName)
  {
    var filePath = Path.Combine(_environment.WebRootPath, UploadsFolderName, fileName);
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
    return Task.CompletedTask;
  }

  // --- INTERFACE ALIAS: DELETE (Calls DeleteFileAsync) ---
  public Task DeleteAsync(string storedFileName)
  {
    return DeleteFileAsync(storedFileName);
  }

  // --- CORE IMPLEMENTATION: GET URL ---
  public string GetFileUrl(string storedFileName)
  {
    // Ensure we return a web-friendly relative path
    return $"/{UploadsFolderName}/{storedFileName}";
  }
}