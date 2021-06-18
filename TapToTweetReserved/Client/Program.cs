using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TapToTweetReserved.Client.Services;
using TapToTweetReserved.Shared;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace TapToTweetReserved.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            var baseUri = builder.HostEnvironment.BaseAddress;

            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(baseUri) });
            builder.Services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddI18nText();

            builder.Services.AddSingleton(services =>
            {
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
                return new ReservedTweets.ReservedTweetsClient(channel);
            });

            await builder.Build().RunAsync();
        }
    }
}
