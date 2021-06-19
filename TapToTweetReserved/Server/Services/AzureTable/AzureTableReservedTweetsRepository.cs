using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services.AzureTable
{
    public class AzureTableReservedTweetsRepository : IReservedTweetsRepository
    {
        private const string TableName = "ReservedTweets";

        private CloudStorageAccount StorageAccount { get; }

        public AzureTableReservedTweetsRepository(IOptions<AzureStorageAccount> storageAccount)
        {
            this.StorageAccount = CloudStorageAccount.Parse(storageAccount.Value.StorageConnectionString);
        }

        private async Task<T> ActionAsync<T>(Func<CloudTable, Task<T>> action)
        {
            var tableClient = this.StorageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();

            var result = await action(table);

            return result;
        }

        public async Task AddAsync(string twitterUserId, string textToTweet)
        {
            await ActionAsync(async table =>
            {
                var allTweets = await GetAllAsync(table, twitterUserId);
                var maxOrder = allTweets.DefaultIfEmpty().Max(e => e?.OriginalEntity.Order ?? 0);

                var newTweet = new ReservedTweet
                {
                    Id = Guid.NewGuid().ToString(),
                    TextToTweet = textToTweet,
                    Order = maxOrder + 1
                };

                var entity = new TableEntityAdapter<ReservedTweet>(newTweet, twitterUserId, newTweet.Id.ToString());
                var tableOperation = TableOperation.Insert(entity);
                await table.ExecuteAsync(tableOperation);

                return newTweet.Id;
            });
        }

        public Task<ReservedTweet[]> GetAllAsync(string twitterUserId)
        {
            return ActionAsync(async table =>
            {
                var allTweets = await GetAllAsync(table, twitterUserId);
                return allTweets.Select(t => t.OriginalEntity).ToArray();
            });
        }

        private static async Task<TableQuerySegment<TableEntityAdapter<ReservedTweet>>> GetAllAsync(CloudTable table, string twitterUserId)
        {
            var query = new TableQuery<TableEntityAdapter<ReservedTweet>>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, twitterUserId));
            var allTweets = await table.ExecuteQuerySegmentedAsync(query, null);
            return allTweets;
        }

        public Task<ReservedTweet> GetAsync(string twitterUserId, string id)
        {
            return ActionAsync(async table =>
            {
                var entity = await GetAsync(table, twitterUserId, id);
                return entity.OriginalEntity;
            });
        }

        private static async Task<TableEntityAdapter<ReservedTweet>> GetAsync(CloudTable table, string twitterUserId, string id)
        {
            var operation = TableOperation.Retrieve<TableEntityAdapter<ReservedTweet>>(twitterUserId, id);
            var res = await table.ExecuteAsync(operation);
            return res.Result as TableEntityAdapter<ReservedTweet>;
        }

        public Task UpdateAsync(string twitterUserId, string id, string textToTweet, int order, bool isTweeted)
        {
            return ActionAsync<object>(async table =>
            {
                var entity = await GetAsync(table, twitterUserId, id);
                entity.OriginalEntity.TextToTweet = textToTweet;
                entity.OriginalEntity.Order = order;
                entity.OriginalEntity.IsTweeted = isTweeted;
                var operation = TableOperation.Replace(entity);
                await table.ExecuteAsync(operation);
                return null;
            });
        }

        public Task DeleteAsync(string twitterUserId, string id)
        {
            return ActionAsync<object>(async (CloudTable table) =>
            {
                var entity = await GetAsync(table, twitterUserId, id);
                var tableOperation = TableOperation.Delete(entity);
                await table.ExecuteAsync(tableOperation);
                return null;
            });
        }
    }
}
