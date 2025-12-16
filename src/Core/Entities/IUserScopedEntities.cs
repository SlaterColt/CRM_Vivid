// FILE: src/Core/Entities/IUserScopedEntity.cs (NEW FILE)
namespace CRM_Vivid.Core.Entities;

/// <summary>
/// Marks an entity that must be scoped/filtered by the creating user's ID.
/// </summary>
public interface IUserScopedEntity
{
  Guid CreatedByUserId { get; set; }
}