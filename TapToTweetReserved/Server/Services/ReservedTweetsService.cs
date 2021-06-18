using System.Threading.Tasks;
using Grpc.Core;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services
{
    public class ReservedTweetsService : ReservedTweets.ReservedTweetsBase
    {
        private TwitterConfiguration TwitterConfiguration { get; }

        private IReservedTweetsRepository ReservedTweetsRepository { get; }

        public ReservedTweetsService(
            TwitterConfiguration twitterConfiguration,
            IReservedTweetsRepository reservedTweetsRepository)
        {
            this.TwitterConfiguration = twitterConfiguration;
            this.ReservedTweetsRepository = reservedTweetsRepository;
        }

        public override async Task<Uuid> AddTweet(ReservedTweet request, ServerCallContext context)
        {
            var twitterUserId = context.GetHttpContext().User.Claims.GetTwitterUserId();
            var newTweetId = await this.ReservedTweetsRepository.AddAsync(twitterUserId, request.TextToTweet);
            return newTweetId;
        }
    }
}
