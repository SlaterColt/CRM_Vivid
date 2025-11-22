using System.IO;
using System.Threading.Tasks;

namespace CRM_Vivid.Application.Common.Interfaces
{
  public interface IFileStorageService
  {
    Task<string> UploadAsync(Stream fileStream, string fileName);
    Task DeleteAsync(string storedFileName);
    string GetFileUrl(string storedFileName);
  }
}