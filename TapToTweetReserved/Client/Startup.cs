using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using TapToTweetReserved.Client.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace TapToTweetReserved.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();
            services.AddAuthorizationCore();
            services.AddI18nText<Startup>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
