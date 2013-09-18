using System;
using System.Linq;
using Microsoft.Web.WebPages.OAuth;
using TapToTweetReserved.Code;

namespace TapToTweetReserved
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            var oauthTwitterSetting = JsonAppSettings.AsDictionary("Twitter.Keys");
            OAuthWebSecurity.RegisterClient(new global::TapToTweetReserved.Code.TwitterClient(
                oauthTwitterSetting["consumerKey"],
                oauthTwitterSetting["consumerSecret"]
                ));
            //OAuthWebSecurity.RegisterTwitterClient(
            //    oauthTwitterSetting["consumerKey"],
            //    oauthTwitterSetting["consumerSecret"]);
        }
    }
}