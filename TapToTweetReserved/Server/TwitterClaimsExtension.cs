using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TapToTweetReserved.Server
{
    public static class TwitterClaimsExtension
    {
        public static string GetTwitterAccessToken(this IEnumerable<Claim> claims)
        {
            return claims.First(c => c.Type == TwitterClaimTypes.AccessToken).Value;
        }

        public static string GetTwitterAccessTokenSecret(this IEnumerable<Claim> claims)
        {
            return claims.First(c => c.Type == TwitterClaimTypes.AccessTokenSecret).Value;
        }
    }
}
