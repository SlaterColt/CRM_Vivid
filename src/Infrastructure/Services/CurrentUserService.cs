// FILE: src/Infrastructure/Services/CurrentUserService.cs (NEW FILE)
// Placeholder implementation
using CRM_Vivid.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CRM_Vivid.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
  // NOTE: This implementation relies on HttpContext and Authentication Middleware (Clerk).
  // For now, we return a simple mock GUID for compilation.
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CurrentUserService(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Guid CurrentUserId
  {
    get
    {
      // In a live app, this would extract the UserId from ClaimsPrincipal:
      // var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("user_id");
      // if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var id)) return id;

      // For now, return a fixed GUID to enable the Global Filter to function
      // (You must ensure this ID matches a user ID when testing)
      return new Guid("11111111-2222-3333-4444-555555555555");
    }
  }

  public bool IsAuthenticated => true; // Assume authenticated for [Authorize] paths
}