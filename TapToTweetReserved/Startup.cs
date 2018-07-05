using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TapToTweetReserved.Startup))]

namespace TapToTweetReserved
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.Use(EnforceHttps);
            app.UseWebApi(config);
        }

        private static async Task EnforceHttps(IOwinContext ctx, Func<Task> next)
        {
            var url = ctx.Request.Uri;
            if (url.Scheme == "http" && url.Host != "localhost")
            {
                ctx.Response.Redirect($"https://{url.Host}/");
            }
            else
            {
                await next();
            }
        }
    }
}
