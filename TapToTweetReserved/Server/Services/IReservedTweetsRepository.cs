using System.Threading.Tasks;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services
{
    public interface IReservedTweetsRepository
    {
        ValueTask AddAsync(string twitterUserId, string textToTweet);

        ValueTask<ReservedTweet[]> GetAllAsync(string twitterUserId);

        ValueTask<ReservedTweet> GetAsync(string twitterUserId, string id);

        ValueTask UpdateAsync(string twitterUserId, string id, string textToTweet, int order, bool isTweeted);

        ValueTask DeleteAsync(string twitterUserId, string id);
    }
}
