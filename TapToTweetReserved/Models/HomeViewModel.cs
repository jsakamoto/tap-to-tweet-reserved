using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TapToTweetReserved.Models
{
    public class HomeViewModel
    {
        public bool NothingReserved { get; set; }

        public IEnumerable<ReservedTweet> ReservedTweets { get; set; }
    }
}