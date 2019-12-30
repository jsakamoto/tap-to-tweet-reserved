using System;

namespace TapToTweetReserved.Shared
{
    public class ReservedTweet
    {
        public Guid Id { get; set; }

        public string TextToTweet { get; set; }

        public int Order { get; set; }

        public bool IsTweeted { get; set; }
    }
}
