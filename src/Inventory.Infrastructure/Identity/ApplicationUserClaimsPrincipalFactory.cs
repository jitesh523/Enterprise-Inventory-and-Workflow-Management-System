using Inventory.Domain.Entities.Identity;
using Inventory.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Inventory.Infrastructure.Identity;

/// <summary>
/// Custom claims principal factory to add permissions as claims
/// </summary>
public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    private readonly ApplicationDbContext _context;

    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> options,
        ApplicationDbContext context)
        : base(userManager, roleManager, options)
    {
        _context = context;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Get all roles for the user
        var roles = await UserManager.GetRolesAsync(user);

        // Get all permissions for these roles
        var permissions = await _context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .Include(rp => rp.Role)
            .Where(rp => roles.Contains(rp.Role!.Name!))
            .Select(rp => rp.Permission!.ClaimValue)
            .Distinct()
            .ToListAsync();

        // Add permissions as claims
        foreach (var permission in permissions)
        {
            identity.AddClaim(new Claim("Permission", permission));
        }

        // Add custom claims
        identity.AddClaim(new Claim("FullName", user.FullName));
        identity.AddClaim(new Claim("IsActive", user.IsActive.ToString()));

        return identity;
    }
}
