using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Client.Services
{
    public class AppAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient HttpClient;

        public AppAuthenticationStateProvider(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await this.HttpClient.GetJsonAsync<AuthUserInfo>("/api/auth/currentuser");
            Console.WriteLine($"userName is [{user?.Name}]");

            var identity = default(ClaimsIdentity);
            if (!string.IsNullOrEmpty(user?.Name))
            {
                identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Name) }, authenticationType: "Twitter");
            }
            Console.WriteLine($"identity is [{JsonSerializer.Serialize(new { identity?.Name, identity?.IsAuthenticated, identity?.AuthenticationType }, new JsonSerializerOptions { WriteIndented = true })}]");

            return new AuthenticationState(new ClaimsPrincipal(identity ?? new ClaimsIdentity()));
        }
    }
}
