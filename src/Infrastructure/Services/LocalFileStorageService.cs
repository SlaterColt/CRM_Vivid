using CRM_Vivid.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace CRM_Vivid.Infrastructure.Services
{
  public class LocalFileStorageService : IFileStorageService
  {
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadsFolder;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
      _environment = environment;

      // FIX: WebRootPath is null if wwwroot doesn't exist or isn't configured.
      // We fallback to combining ContentRootPath (project root) with "wwwroot".
      var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

      _uploadsFolder = Path.Combine(webRoot, "uploads");

      // Ensure the directory hierarchy exists
      if (!Directory.Exists(_uploadsFolder))
      {
        Directory.CreateDirectory(_uploadsFolder);
      }
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
      var extension = Path.GetExtension(fileName);
      var uniqueFileName = $"{Guid.NewGuid()}{extension}";
      var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

      using (var targetStream = new FileStream(filePath, FileMode.Create))
      {
        await fileStream.CopyToAsync(targetStream);
      }

      return uniqueFileName;
    }

    public Task DeleteAsync(string storedFileName)
    {
      var filePath = Path.Combine(_uploadsFolder, storedFileName);
      if (File.Exists(filePath))
      {
        File.Delete(filePath);
      }
      return Task.CompletedTask;
    }

    public string GetFileUrl(string storedFileName)
    {
      return $"/uploads/{storedFileName}";
    }
  }
}