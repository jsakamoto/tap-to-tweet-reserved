using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Client
{
    public class SelectableReservedTweet
    {
        public bool Selected;

        public readonly ReservedTweet Message;

        public SelectableReservedTweet(ReservedTweet message)
        {
            Message = message;
        }
    }
}
