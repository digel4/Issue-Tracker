using System.Security.Claims;
using IssueTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IssueTracker.Services.Factories;

public class ITUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ITUser, IdentityRole>
{
    public ITUserClaimsPrincipalFactory(
        UserManager<ITUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager,roleManager,optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ITUser user)
    {
        ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));

        return identity;
    }
}