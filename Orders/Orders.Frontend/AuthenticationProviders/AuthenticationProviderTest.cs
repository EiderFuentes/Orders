using Microsoft.AspNetCore.Components.Authorization;
using Orders.Shared.Entities;
using System.Security.Claims;

namespace Orders.Frontend.AuthenticationProviders
{
    public class AuthenticationProviderTest : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.Delay(1500);
            var anonimous = new ClaimsIdentity();//Usuario Anonimo
            var user = new ClaimsIdentity(authenticationType: "test");//Usuario User
            var admin = new ClaimsIdentity(new List<Claim>// Usuario admin
            {
                new Claim("FirstName", "Eider"),
                new Claim("LastName", "Fuentes"),
                new Claim(ClaimTypes.Name, "eider@yopmail.com"),
                new Claim(ClaimTypes.Role, "Admin")
            },
             authenticationType: "test");

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(user)));
        }
    }
}
