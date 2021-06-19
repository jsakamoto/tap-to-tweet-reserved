using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TapToTweetReserved.Server.Services;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Controllers
{
    [Authorize]
    public class ReservedTweetsController : Controller
    {
        private TwitterConfiguration TwitterConfiguration { get; }

        private IReservedTweetsRepository ReservedTweetsRepository { get; }

        public ReservedTweetsController(
            TwitterConfiguration twitterConfiguration,
            IReservedTweetsRepository reservedTweetsRepository)
        {
            TwitterConfiguration = twitterConfiguration;
            ReservedTweetsRepository = reservedTweetsRepository;
        }

        [HttpGet("/api/reservedtweets")]
        public async Task<ReservedTweet[]> GetAllAsync()
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            var reservedTweets = await this.ReservedTweetsRepository.GetAllAsync(twitterUserId);
            return reservedTweets.OrderBy(t => t.Order).ToArray();
        }

        [HttpGet("/api/reservedtweets/{id}")]
        public async Task<ReservedTweet> GetAsync(string id)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            var reservedTweet = await this.ReservedTweetsRepository.GetAsync(twitterUserId, id);
            return reservedTweet;
        }

        [HttpPut("/api/reservedtweets/{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] ReservedTweet tweet)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            await this.ReservedTweetsRepository.UpdateAsync(twitterUserId, id,
                tweet.TextToTweet,
                tweet.Order,
                tweet.IsTweeted);
            return Ok();
        }

        [HttpDelete("/api/reservedtweets/{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            await this.ReservedTweetsRepository.DeleteAsync(twitterUserId, id);
            return Ok();
        }

        [HttpPost("/api/reservedtweets/{id}/tweet")]
        public async Task<IActionResult> PostTweetAsync(string id)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            var reservedTweet = await this.ReservedTweetsRepository.GetAsync(twitterUserId, id);
            if (reservedTweet == null) return NotFound();

            var token = CoreTweet.Tokens.Create(
                TwitterConfiguration.ConsumerAPIKey,
                TwitterConfiguration.ConsumerSecret,
                this.User.Claims.GetTwitterAccessToken(),
                this.User.Claims.GetTwitterAccessTokenSecret());
            await token.Statuses.UpdateAsync(reservedTweet.TextToTweet);

            return Ok();
        }
    }
}
