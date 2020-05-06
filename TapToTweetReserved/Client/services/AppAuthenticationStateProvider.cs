using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
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
            var user = await this.HttpClient.GetFromJsonAsync<AuthUserInfo>("/api/auth/currentuser");
            var identity = default(ClaimsIdentity);
            if (!string.IsNullOrEmpty(user?.Name))
            {
                identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Name) }, authenticationType: "Twitter");
            }
            return new AuthenticationState(new ClaimsPrincipal(identity ?? new ClaimsIdentity()));
        }

        public Task<AuthenticationState> RefreshAsync()
        {
            var stateAsync = this.GetAuthenticationStateAsync();
            this.NotifyAuthenticationStateChanged(stateAsync);
            return stateAsync;
        }
    }
}
