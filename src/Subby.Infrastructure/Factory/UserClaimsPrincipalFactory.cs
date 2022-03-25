using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Subby.Core.Entities;

namespace Subby.Infrastructure.Factory
{
    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        public UserClaimsPrincipalFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor): base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            identity.AddClaims(new Claim[]
            {
                new Claim("Role", user.Role),
                new Claim("FullName", user.UserName),
                new Claim("Id", user.Id.ToString())
            });

            return identity;
        }
    }
}