using System.Security.Claims;

namespace bentley.Api.Security
{
    public static class FormAuthorization
    {

        public static bool UserCanView(ClaimsPrincipal user)
        {
            return true;
        }

        public static bool UserCanModify(ClaimsPrincipal user)
        {
            return true;
        }
            
    }
}
