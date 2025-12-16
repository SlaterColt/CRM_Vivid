// FILE: src/Application/Common/Interfaces/ICurrentUserService.cs (NEW FILE)
namespace CRM_Vivid.Application.Common.Interfaces;

/// <summary>
/// Provides access to the current authenticated user's ID.
/// </summary>
public interface ICurrentUserService
{
  Guid CurrentUserId { get; }
  bool IsAuthenticated { get; }
}