using System;
using System.Linq;
using System.Web.Http;
using TapToTweetReserved.Models;
using WebApi.OutputCache.V2;

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

        [CacheOutput(NoCache = true)]
        public ReservedTweet[] Get()
        {
            var userExtraData = this.User.ExtraData<UserExtraData>();
            var userId = userExtraData.UserId;
            return Db.ReservedTweets
                .Where(t => t.OwnerUserId == userId)
                .OrderBy(t => t.Order)
                .ToArray();
        }

        [CacheOutput(NoCache = true)]
        public ReservedTweet Get(int id)
        {
            return GetTargetTweet(id);
        }

        public void Delete(int id)
        {
            var reservedTweet = GetTargetTweet(id);
            Db.ReservedTweets.Remove(reservedTweet);
            Db.SaveChanges();
        }

        public ReservedTweet Post(ReservedTweet tweet)
        {
            var userExtraData = this.User.ExtraData<UserExtraData>();
            var userId = userExtraData.UserId;
            var newTweet = new ReservedTweet
            {
                OwnerUserId = userId,
                TextToTweet = tweet.TextToTweet,
                Order = Db.ReservedTweets.Select(t => t.Order).DefaultIfEmpty().Max() + 1
            };
            Db.ReservedTweets.Add(newTweet);
            Db.SaveChanges();

            return newTweet;
        }

        public void Post(int id, ReservedTweet tweet)
        {
            var target = GetTargetTweet(id);
            target.Order = tweet.Order;
            target.TextToTweet = tweet.TextToTweet;
            target.IsTweeted = tweet.IsTweeted;
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
