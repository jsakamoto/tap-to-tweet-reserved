using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
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

        private ReservedTweet GetTargetTweet(int id)
        {
            var userExtraData = this.User.ExtraData<UserExtraData>();
            var userId = userExtraData.UserId;
            var reservedTweet = Db.ReservedTweets
                .Where(t => t.OwnerUserId == userId)
                .FirstOrDefault(t => t.Id == id);
            if (reservedTweet == null) throw new ArgumentException("reserved tweet not found find by specified id.", "id");
            return reservedTweet;
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var reservedTweet = GetTargetTweet(id);
            Db.ReservedTweets.Remove(reservedTweet);
            Db.SaveChanges();

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var reservedTweet = GetTargetTweet(id);
            return View(reservedTweet);
        }

        [HttpPost]
        public ActionResult Edit(int id, ReservedTweet model)
        {
            if (ModelState.IsValid == false) return View(model);

            var reservedTweet = GetTargetTweet(id);
            UpdateModel(reservedTweet, includeProperties: new[] { "TextToTweet" });
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Up(int id)
        {
            UpOrDown(id, direction: -1);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Down(int id)
        {
            UpOrDown(id, direction: +1);
            return new EmptyResult();
        }

        private void UpOrDown(int id, int direction)
        {
            if (this.Request.IsAjaxRequest() == false) throw new HttpException("does not ajax request.");

            var userExtraData = this.User.ExtraData<UserExtraData>();
            var userId = userExtraData.UserId;
            var reservedTweets = Db.ReservedTweets
                .Where(t => t.OwnerUserId == userId)
                .OrderBy(t => t.Order).ToList();
            var targetTweet = reservedTweets.FirstOrDefault(t => t.Id == id);
            if (targetTweet == null) throw new ArgumentException("reserved tweet not found find by specified id.", "id");

            var swapToIndex = reservedTweets.IndexOf(targetTweet) + direction;
            if (swapToIndex < 0 || reservedTweets.Count <= swapToIndex) return;
            var swapTo = reservedTweets[swapToIndex];

            var tmp = targetTweet.Order;
            targetTweet.Order = swapTo.Order;
            swapTo.Order = tmp;

            Db.SaveChanges();
        }

        [HttpPost]
        public ActionResult Reload(int id)
        {
            var reservedTweet = GetTargetTweet(id);
            reservedTweet.IsTweeted = false;
            Db.SaveChanges();

            return new EmptyResult();
        }
    }
}
