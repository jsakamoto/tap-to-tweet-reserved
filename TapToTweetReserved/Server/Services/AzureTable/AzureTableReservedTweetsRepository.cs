using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services.AzureTable
{
    public class AzureTableReservedTweetsRepository : IReservedTweetsRepository
    {
        private const string TableName = "ReservedTweets";

        private TableClient TableClient { get; }

        public AzureTableReservedTweetsRepository(IOptions<AzureStorageAccount> storageAccount)
        {
            var tableServiceClient = new TableServiceClient(storageAccount.Value.StorageConnectionString);
            this.TableClient = tableServiceClient.GetTableClient(TableName);
            this.TableClient.CreateIfNotExists();
        }

        public async ValueTask AddAsync(string twitterUserId, string textToTweet)
        {
            var allTweets = await this.GetAllAsync(twitterUserId);
            var maxOrder = allTweets.DefaultIfEmpty().Max(e => e?.Order ?? 0);

            var newTweetId = Guid.NewGuid().ToString();
            var newTweet = new ReservedTweet
            {
                PartitionKey = twitterUserId,
                RowKey = newTweetId,
                Id = newTweetId,
                TextToTweet = textToTweet,
                Order = maxOrder + 1
            };
            await this.TableClient.AddEntityAsync(newTweet);
        }

        public ValueTask<ReservedTweet[]> GetAllAsync(string twitterUserId)
        {
            var allTweets = this.TableClient.Query<ReservedTweet>(t => t.PartitionKey == twitterUserId).ToArray();
            return ValueTask.FromResult(allTweets);
        }

        public async ValueTask<ReservedTweet> GetAsync(string twitterUserId, string id)
        {
            var res = await this.TableClient.GetEntityAsync<ReservedTweet>(twitterUserId, id);
            return res.Value;
        }

        public async ValueTask UpdateAsync(string twitterUserId, string id, string textToTweet, int order, bool isTweeted)
        {
            var entity = await this.GetAsync(twitterUserId, id);
            entity.TextToTweet = textToTweet;
            entity.Order = order;
            entity.IsTweeted = isTweeted;
            await this.TableClient.UpdateEntityAsync(entity, entity.ETag);
        }

        public async ValueTask DeleteAsync(string twitterUserId, string id)
        {
            await this.TableClient.DeleteEntityAsync(twitterUserId, id);
        }
    }
}
