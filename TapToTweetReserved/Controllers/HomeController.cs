using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TapToTweetReserved.Code;
using TapToTweetReserved.Models;
using TweetSharp;

namespace TapToTweetReserved.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public TapToTweetReservedDb Db { get; set; }

        public HomeController()
        {
            this.Db = new TapToTweetReservedDb();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var userExtraData = this.User.ExtraData<UserExtraData>();
            var userId = userExtraData.UserId;
            var model = new HomeViewModel
            {
                ReservedTweets = Db.ReservedTweets
                    .Where(t => t.OwnerUserId == userId)
                    .Where(t => t.IsTweeted == false)
                    .OrderBy(t => t.Order).ToArray()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Tweet(int id)
        {
            if (this.Request.IsAjaxRequest() == false) throw new HttpException("does not ajax request.");
            var userExtraData = this.User.ExtraData<UserExtraData>();

            var userId = userExtraData.UserId;
            var tweet = Db.ReservedTweets.FirstOrDefault(t => t.Id == id && t.OwnerUserId == userId);
            if (tweet == null) throw new ArgumentException("stocked tweet not found find by specified id.", "id");

            if (tweet.IsTweeted == false)
            {
                var keys = JsonAppSettings.AsDictionary("Twitter.Keys");
                if (keys != null)
                {
                    var service = new TwitterService(keys["consumerKey"], keys["consumerSecret"]);
                    service.AuthenticateWith(userExtraData.AccessToken, userExtraData.AccessSecret);
                    service.SendTweet(new SendTweetOptions { Status = tweet.TextToTweet });
                    tweet.IsTweeted = true;
                    Db.SaveChanges();
                }
            }
            return new EmptyResult();
        }
    }
}
