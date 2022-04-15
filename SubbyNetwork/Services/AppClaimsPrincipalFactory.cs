using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Subby.Data;

namespace SubbyNetwork.Services
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
    {
        public AppClaimsPrincipalFactory(
            UserManager<User> userManager
            , RoleManager<Role> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.GivenName, user.FirstName)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.Surname, user.LastName),
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.Role, user.Role),
                });
            }

            ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                new Claim("Id", user.Id.ToString()),
            });

            return principal;
        }
    }
}
