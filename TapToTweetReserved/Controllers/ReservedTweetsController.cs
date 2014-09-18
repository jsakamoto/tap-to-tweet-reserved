using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TapToTweetReserved.Models;

namespace TapToTweetReserved.Controllers
{
    [Authorize]
    public class ReservedTweetsController : ApiController
    {
        public TapToTweetReservedDb Db { get; set; }

        public ReservedTweetsController()
        {
            this.Db = new TapToTweetReservedDb();
        }

        public ReservedTweet[] Get()
        {
            var userExtraData = this.User.ExtraData<UserExtraData>();
            var userId = userExtraData.UserId;
            return Db.ReservedTweets
                .Where(t => t.OwnerUserId == userId)
                .OrderBy(t => t.Order)
                .ToArray();
        }

        public void Delete(int id)
        {
            var reservedTweet = GetTargetTweet(id);
            Db.ReservedTweets.Remove(reservedTweet);
            Db.SaveChanges();
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
    }
}
