﻿using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using TapToTweetReserved.Server.Services;
using TapToTweetReserved.Server.Services.AzureTable;
using TapToTweetReserved.Server.Services.LocalFile;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Toolbelt.Extensions.DependencyInjection;

namespace TapToTweetReserved.Server;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        var twitterConfig = new TwitterConfiguration();
        this.Configuration.Bind("Authentication:Twitter", twitterConfig);
        services.AddSingleton(twitterConfig);
        services.Configure<AzureStorageAccount>(this.Configuration.GetSection("AzureStorageAccount"));

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddGrpc();
        services.AddRazorPages();
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new ResponseCacheAttribute { NoStore = true });
        });

        services.AddScoped(sp => new HttpClient());
        services.AddScoped<AuthenticationStateProvider, DummyAuthenticationStateProvider>();
        ConfigureAuthentication(services, twitterConfig);

        var repositoryType = this.Configuration.GetValue("ReservedTweetRepository:Type", "AzureTable");
        switch (repositoryType)
        {
            case "AzureTable":
                services.AddScoped<IReservedTweetsRepository, AzureTableReservedTweetsRepository>();
                break;
            case "LocalFile":
                services.AddSingleton<IReservedTweetsRepository, LocalFileReservedTweetsRepository>();
                break;
            default: throw new Exception($"Unknown repository type: \"{repositoryType}\"");
        }
        services.AddI18nText();
    }

    private static void ConfigureAuthentication(IServiceCollection services, TwitterConfiguration twitterConfig)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddTwitter(options =>
            {
                options.ConsumerKey = twitterConfig.ConsumerAPIKey;
                options.ConsumerSecret = twitterConfig.ConsumerSecret;
                options.Events.OnCreatingTicket = context =>
                {
                    var identity = context.Principal?.Identity as ClaimsIdentity;
                    if (identity == null) throw new Exception("context.Principal is null or context.Principal.Identity is not ClaimsIdentity.");
                    identity.AddClaim(new Claim(TwitterClaimTypes.AccessToken, context.AccessToken));
                    identity.AddClaim(new Claim(TwitterClaimTypes.AccessTokenSecret, context.AccessTokenSecret));
                    return Task.CompletedTask;
                };
                options.Events.OnRemoteFailure = context =>
                {
                    var appUrl = $"{context.Request.Scheme}://{context.Request.Host}/";
                    context.Response.Redirect(appUrl);
                    context.HandleResponse();
                    return Task.CompletedTask;
                };
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.Strict;

                // https://stackoverflow.com/a/44600389/1268000
                options.Events.OnRedirectToLogin =
                options.Events.OnRedirectToAccessDenied = async (ctx) =>
                {
                    ctx.Response.StatusCode = 403;
                    var message = Encoding.UTF8.GetBytes("Access Denied.");
                    await ctx.Response.Body.WriteAsync(message, 0, message.Length);
                };
            });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
            app.UseCssLiveReload();
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseGrpcWeb();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<ReservedTweetsService>().EnableGrpcWeb();
            endpoints.MapRazorPages();
            endpoints.MapDefaultControllerRoute();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
