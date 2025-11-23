using Microsoft.AspNetCore.Authorization;

namespace Inventory.Web.Attributes;

/// <summary>
/// Custom authorization attribute for permission-based access control
/// </summary>
public class AuthorizePermissionAttribute : AuthorizeAttribute
{
    public AuthorizePermissionAttribute(string permission)
    {
        Policy = permission;
    }
}
