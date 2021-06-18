using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services.LocalFile
{
    public class LocalFileReservedTweetsRepository : IReservedTweetsRepository
    {
        public Dictionary<string, List<ReservedTweet>> ReservedTweets { get; set; }

        private SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);

        private IWebHostEnvironment HostEnvironment { get; }

        public LocalFileReservedTweetsRepository(IWebHostEnvironment hostEnvironment)
        {
            HostEnvironment = hostEnvironment;
        }

        private async Task<string> GetStoragePathAsync()
        {
            var appDataDir = Path.Combine(HostEnvironment.ContentRootPath, "App_Data");
            if (!Directory.Exists(appDataDir)) Directory.CreateDirectory(appDataDir);
            var storagePath = Path.Combine(appDataDir, "ReservedTweets.json");
            if (!File.Exists(storagePath))
            {
                await File.WriteAllTextAsync(storagePath, "{}");
            }
            return storagePath;
        }

        private async Task<T> ActionAsync<T>(Func<T> action)
        {
            await Lock.WaitAsync();
            try
            {
                var storagePath = await GetStoragePathAsync();
                if (this.ReservedTweets == null)
                {
                    var initialJson = await File.ReadAllTextAsync(storagePath);
                    this.ReservedTweets = JsonSerializer.Deserialize<Dictionary<string, List<ReservedTweet>>>(initialJson);
                }

                var result = action();

                var json = JsonSerializer.Serialize(this.ReservedTweets);
                await File.WriteAllTextAsync(storagePath, json);

                return result;
            }
            finally { Lock.Release(); }
        }

        public Task<Uuid> AddAsync(string twitterUserId, string textToTweet)
        {
            return ActionAsync(() =>
            {
                var tweets = default(List<ReservedTweet>);
                if (!this.ReservedTweets.TryGetValue(twitterUserId, out tweets))
                {
                    tweets = new List<ReservedTweet>();
                    this.ReservedTweets.Add(twitterUserId, tweets);
                }

                var newTweet = new ReservedTweet
                {
                    Id = Uuid.NewUuid(),
                    TextToTweet = textToTweet,
                    Order = tweets.DefaultIfEmpty().Max(t => t?.Order ?? 0) + 1
                };
                tweets.Add(newTweet);

                return newTweet.Id;
            });
        }

        public Task<ReservedTweet[]> GetAllAsync(string twitterUserId)
        {
            return ActionAsync(() =>
            {
                if (this.ReservedTweets.TryGetValue(twitterUserId, out var tweets))
                    return tweets.ToArray();
                return new ReservedTweet[0];
            });
        }

        public Task<ReservedTweet> GetAsync(string twitterUserId, Guid id)
        {
            return ActionAsync(() =>
            {
                if (!this.ReservedTweets.TryGetValue(twitterUserId, out var tweets)) return null;
                return tweets.FirstOrDefault(t => t.Id.Value == id.ToString());
            });
        }

        public Task UpdateAsync(string twitterUserId, Guid id, string textToTweet, int order, bool isTweeted)
        {
            return ActionAsync<object>(() =>
            {
                if (!this.ReservedTweets.TryGetValue(twitterUserId, out var tweets)) return null;
                var targetTweet = tweets.FirstOrDefault(t => t.Id.Value == id.ToString());
                if (targetTweet != null)
                {
                    targetTweet.TextToTweet = textToTweet;
                    targetTweet.Order = order;
                    targetTweet.IsTweeted = isTweeted;
                }
                return null;
            });
        }

        public Task DeleteAsync(string twitterUserId, Guid id)
        {
            return ActionAsync<object>(() =>
            {
                if (this.ReservedTweets.TryGetValue(twitterUserId, out var tweets))
                    tweets.RemoveAll(t => t.Id.Value == id.ToString());
                return null;
            });
        }
    }
}
