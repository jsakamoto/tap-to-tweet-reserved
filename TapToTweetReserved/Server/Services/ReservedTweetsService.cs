using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
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

        public override async Task<Empty> AddTweet(ReservedTweet request, ServerCallContext context)
        {
            var twitterUserId = context.GetHttpContext().User.Claims.GetTwitterUserId();
            await this.ReservedTweetsRepository.AddAsync(twitterUserId, request.TextToTweet);
            return new Empty();
        }
    }
}
