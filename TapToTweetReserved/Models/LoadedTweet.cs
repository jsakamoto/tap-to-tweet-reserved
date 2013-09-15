using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TapToTweetReserved.Models
{
    public class LoadedTweet
    {
        public int Id { get; set; }

        public string TextToTweet { get; set; }

        public int Order { get; set; }

        public bool IsTweeted { get; set; }
    }
}