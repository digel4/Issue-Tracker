using System.Security.Claims;
using System.Security.Principal;

namespace IssueTracker.Extensions;

// static means that there is only a single instance of the class in the entire application.
public static class IdentityExtensions
{
    public static int? GetCompanyId(this IIdentity identity)
    {
        Claim claim = ((ClaimsIdentity)identity).FindFirst("CompanyId");

        return (claim != null) ? int.Parse(claim.Value) : null;
    }
}