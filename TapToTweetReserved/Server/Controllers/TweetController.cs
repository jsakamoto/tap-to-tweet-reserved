using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Controllers
{
    [Authorize]
    public class TweetController : Controller
    {
        public TweetController()
        {
        }

        [HttpPost("/api/tweet")]
        public IActionResult PostTweetAsync(Tweet tweet)
        {
            return Ok();
        }
    }
}
