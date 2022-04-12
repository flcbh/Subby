using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SubbyNetwork.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public void IdentityAuthenticationStateProvider()
        {
        }

        public async Task Login(string token)
        {
            await SecureStorage.SetAsync("accounttoken", token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public Task Logout()
        {
            SecureStorage.Remove("accounttoken");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return Task.CompletedTask;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            try
            {
                var userInfo = await SecureStorage.GetAsync("accounttoken");
                if (userInfo != null)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, "ffUser") };
                    identity = new ClaimsIdentity(claims, "Server authentication");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}

