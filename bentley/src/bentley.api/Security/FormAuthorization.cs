using bentley.api.Models;
using System.Security.Claims;

namespace bentley.api.Security
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
