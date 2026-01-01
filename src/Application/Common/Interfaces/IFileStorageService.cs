namespace CRM_Vivid.Application.Common.Interfaces
{
  public interface IFileStorageService
  {
    Task<string> UploadAsync(Stream fileStream, string fileName);
    Task<string> SaveFileAsync(Stream fileStream, string fileName);
    Task<string> SaveFileAsync(byte[] fileBytes, string fileName);
    Task DeleteFileAsync(string fileName);
    Task DeleteAsync(string storedFileName);
    string GetFileUrl(string storedFileName);
  }
}