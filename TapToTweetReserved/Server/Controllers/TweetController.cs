using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Controllers
{
    [Authorize]
    public class TweetController : Controller
    {
        private TwitterConfiguration TwitterConfiguration { get; }

        public TweetController(TwitterConfiguration twitterConfiguration)
        {
            TwitterConfiguration = twitterConfiguration;
        }

        [HttpPost("/api/tweet")]
        public async Task<IActionResult> PostTweetAsync([FromBody]Tweet tweet)
        {
            var token = CoreTweet.Tokens.Create(
                TwitterConfiguration.ConsumerAPIKey,
                TwitterConfiguration.ConsumerSecret,
                this.User.Claims.GetTwitterAccessToken(),
                this.User.Claims.GetTwitterAccessTokenSecret());
            var res = await token.Statuses.UpdateAsync(tweet.Message);

            return Ok();
        }
    }
}
