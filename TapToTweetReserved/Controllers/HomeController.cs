using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TapToTweetReserved.Code;
using TapToTweetReserved.Models;
using TweetSharp;

namespace TapToTweetReserved.Controllers
{
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
            var model = new HomeViewModel
            {
                LoadedTweets = Db.LoadedTweets
                    .Where(t => t.IsTweeted == false)
                    .OrderBy(t => t.Order).ToArray()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Tweet(int id)
        {
            if (this.Request.IsAjaxRequest() == false) throw new HttpException("does not ajax request.");

            var tweet = Db.LoadedTweets.Find(id);
            if (tweet.IsTweeted == false)
            {
                var keys = JsonAppSettings.AsDictionary("Twitter.Keys");
                if (keys != null)
                {
                    var service = new TwitterService(keys["consumerKey"], keys["consumerSecret"]);
                    service.AuthenticateWith(keys["token"], keys["tokenSecret"]);
                    service.SendTweet(new SendTweetOptions { Status = tweet.TextToTweet });
                    tweet.IsTweeted = true;
                    Db.SaveChanges();
                }
            }
            return new EmptyResult();
        }
    }
}
