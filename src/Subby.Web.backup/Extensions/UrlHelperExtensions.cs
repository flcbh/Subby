using Microsoft.AspNetCore.Mvc;
using Subby.Web.Controllers;

namespace Subby.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new {userId, code},
                protocol: scheme);
        }
        
        public static string JobApplicationLink(this IUrlHelper urlHelper, int jobId, string scheme)
        {
            return urlHelper.Action(
                action: nameof(ApplicationsController.Index),
                controller: "Applications",
                values: new {jobId},
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code,
            string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new {userId, code},
                protocol: scheme);
        }
    }
}