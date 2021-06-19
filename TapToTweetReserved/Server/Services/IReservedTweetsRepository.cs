using System;
using System.Threading.Tasks;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services
{
    public interface IReservedTweetsRepository
    {
        Task AddAsync(string twitterUserId, string textToTweet);

        Task<ReservedTweet[]> GetAllAsync(string twitterUserId);

        Task<ReservedTweet> GetAsync(string twitterUserId, string id);

        Task UpdateAsync(string twitterUserId, string id, string textToTweet, int order, bool isTweeted);

        Task DeleteAsync(string twitterUserId, string id);
    }
}
