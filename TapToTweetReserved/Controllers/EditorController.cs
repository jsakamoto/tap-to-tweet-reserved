using System;
using System.Linq;
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
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
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

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var reservedTweet = GetTargetTweet(id);
            return View(reservedTweet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string cmd, ReservedTweet model)
        {
            if (cmd == "OK")
            {
                if (ModelState.IsValid == false) return View(model);

                var reservedTweet = GetTargetTweet(id);
                UpdateModel(reservedTweet, includeProperties: new[] { "TextToTweet" });
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
