using Microsoft.AspNetCore.Identity;

namespace Inventory.Domain.Entities.Identity;

/// <summary>
/// Extended application user with additional properties
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// Extended application role
/// </summary>
public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

/// <summary>
/// Permission entity for granular access control
/// </summary>
public class Permission : BaseEntity
{
    public string ClaimValue { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation property
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

/// <summary>
/// Junction table linking roles to permissions
/// </summary>
public class RolePermission : BaseEntity
{
    public string RoleId { get; set; } = string.Empty;
    public int PermissionId { get; set; }
    
    // Navigation properties
    public ApplicationRole? Role { get; set; }
    public Permission? Permission { get; set; }
}
