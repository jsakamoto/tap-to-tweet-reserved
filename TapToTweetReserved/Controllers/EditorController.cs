using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TapToTweetReserved.Models;

namespace TapToTweetReserved.Controllers
{
    [Authorize]
    public class EditorController : Controller
    {
        public TapToTweetReservedDb Db { get; set; }

        public EditorController()
        {
            this.Db = new TapToTweetReservedDb();
        }

        public ActionResult Index()
        {
            var userExtraData = this.User.ExtraData<UserExtraData>();
            var userId = userExtraData.UserId;
            var model = new HomeViewModel
            {
                ReservedTweets = Db.ReservedTweets
                    .Where(t => t.OwnerUserId == userId)
                    .OrderBy(t => t.Order).ToArray()
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ReservedTweet model, string cmd)
        {
            if (cmd == "OK")
            {
                if (ModelState.IsValid == false)
                {
                    return View(model);
                }

                var userExtraData = this.User.ExtraData<UserExtraData>();
                var userId = userExtraData.UserId;
                var reservedTweets = Db.ReservedTweets.Where(t => t.OwnerUserId == userId).ToArray();
                var nextOrder = reservedTweets.Any() ? reservedTweets.Max(t => t.Order) + 1 : 1;
                Db.ReservedTweets.Add(new ReservedTweet
                {
                    OwnerUserId = userId,
                    Order = nextOrder,
                    TextToTweet = model.TextToTweet
                });
                Db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var userExtraData = this.User.ExtraData<UserExtraData>();
            var userId = userExtraData.UserId;
            var reservedTweet = Db.ReservedTweets
                .Where(t => t.OwnerUserId == userId)
                .FirstOrDefault(t => t.Id == id);
            if (reservedTweet == null) throw new ArgumentException("reserved tweet not found find by specified id.", "id");

            Db.ReservedTweets.Remove(reservedTweet);
            Db.SaveChanges();

            return new EmptyResult();
        }
    }
}
