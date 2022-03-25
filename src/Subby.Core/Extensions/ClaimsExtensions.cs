using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Subby.Core.Extensions
{
    public static class ClaimsExtensions
    {
        public static bool Sandbox(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("sandbox");
            // Test for null to avoid issues during local testing
            return (claim != null) && Convert.ToBoolean(claim.Value);
        }

        public static int GetApplicationId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("app-id");
            // Test for null to avoid issues during local testing
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }

        public static string GetApplicationName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("app-name");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}