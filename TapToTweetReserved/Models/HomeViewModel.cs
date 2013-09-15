using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TapToTweetReserved.Models
{
    public class HomeViewModel
    {
        public IEnumerable<LoadedTweet> LoadedTweets { get; set; }
    }
}