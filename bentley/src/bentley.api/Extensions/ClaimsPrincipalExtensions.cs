using System.Security.Claims;

namespace bentley.api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserName(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? user.FindFirstValue("name");
    }
}
