using System.Linq;
using Grpc.Core;

namespace TapToTweetReserved.Server
{
    public static class TwitterClaimsExtension
    {
        private static string GetClaimValueOf(this ServerCallContext context, string claimType)
        {
            return context.GetHttpContext().User.Claims.First(c => c.Type == claimType).Value;
        }

        public static string GetTwitterUserId(this ServerCallContext context)
        {
            return context.GetClaimValueOf(TwitterClaimTypes.UserId);
        }

        public static string GetTwitterAccessToken(this ServerCallContext context)
        {
            return context.GetClaimValueOf(TwitterClaimTypes.AccessToken);
        }

        public static string GetTwitterAccessTokenSecret(this ServerCallContext context)
        {
            return context.GetClaimValueOf(TwitterClaimTypes.AccessTokenSecret);
        }
    }
}
