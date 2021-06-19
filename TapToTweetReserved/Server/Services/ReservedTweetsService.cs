using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services
{
    [Authorize]
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

        public override async Task<Empty> Add(ReservedTweet request, ServerCallContext context)
        {
            var twitterUserId = context.GetTwitterUserId();
            await this.ReservedTweetsRepository.AddAsync(twitterUserId, request.TextToTweet);
            return new Empty();
        }

        public override async Task<ReservedTweetCollection> GetAll(Empty request, ServerCallContext context)
        {
            var twitterUserId = context.GetTwitterUserId();
            var reservedTweets = await this.ReservedTweetsRepository.GetAllAsync(twitterUserId);
            var items = reservedTweets.OrderBy(t => t.Order).ToArray();
            var collection = new ReservedTweetCollection();
            collection.Items.AddRange(items);
            return collection;
        }

        public override async Task<ReservedTweet> Get(ReservedTweetId request, ServerCallContext context)
        {
            var twitterUserId = context.GetTwitterUserId();
            var reservedTweet = await this.ReservedTweetsRepository.GetAsync(twitterUserId, request.Id);
            return reservedTweet;
        }

        public override async Task<Empty> Update(ReservedTweet request, ServerCallContext context)
        {
            var twitterUserId = context.GetTwitterUserId();
            await this.ReservedTweetsRepository.UpdateAsync(twitterUserId,
                request.Id,
                request.TextToTweet,
                request.Order,
                request.IsTweeted);
            return new Empty();
        }

        public override async Task<Empty> Delete(ReservedTweetId request, ServerCallContext context)
        {
            var twitterUserId = context.GetTwitterUserId();
            await this.ReservedTweetsRepository.DeleteAsync(twitterUserId, request.Id);
            return new Empty();
        }

        public override async Task<Empty> Tweet(ReservedTweetId request, ServerCallContext context)
        {
            var twitterUserId = context.GetTwitterUserId();
            var reservedTweet = await this.ReservedTweetsRepository.GetAsync(twitterUserId, request.Id);
            if (reservedTweet == null) throw new RpcException(new Status(StatusCode.NotFound, "The message was not found."));

            var token = CoreTweet.Tokens.Create(
                TwitterConfiguration.ConsumerAPIKey,
                TwitterConfiguration.ConsumerSecret,
                context.GetTwitterAccessToken(),
                context.GetTwitterAccessTokenSecret());
            await token.Statuses.UpdateAsync(reservedTweet.TextToTweet);

            return new Empty();
        }
    }
}
