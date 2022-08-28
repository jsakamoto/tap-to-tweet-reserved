using System.Text.Json;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services.LocalFile;

public class LocalFileReservedTweetsRepository : IReservedTweetsRepository
{
    public class ReservedTweetDictionary : Dictionary<string, List<ReservedTweet>> { }

    public ReservedTweetDictionary? ReservedTweets { get; set; }

    private SemaphoreSlim Lock { get; } = new(1, 1);

    private IWebHostEnvironment HostEnvironment { get; }

    public LocalFileReservedTweetsRepository(IWebHostEnvironment hostEnvironment)
    {
        this.HostEnvironment = hostEnvironment;
    }

    private async ValueTask<string> GetStoragePathAsync()
    {
        var appDataDir = Path.Combine(this.HostEnvironment.ContentRootPath, "App_Data");
        if (!Directory.Exists(appDataDir)) Directory.CreateDirectory(appDataDir);
        var storagePath = Path.Combine(appDataDir, "ReservedTweets.json");
        if (!File.Exists(storagePath))
        {
            await File.WriteAllTextAsync(storagePath, "{}");
        }
        return storagePath;
    }

    private async ValueTask<T?> ActionAsync<T>(Func<ReservedTweetDictionary, T?> action)
    {
        await this.Lock.WaitAsync();
        try
        {
            var storagePath = await this.GetStoragePathAsync();
            if (this.ReservedTweets == null)
            {
                var initialJson = await File.ReadAllTextAsync(storagePath);
                this.ReservedTweets = JsonSerializer.Deserialize<ReservedTweetDictionary>(initialJson) ?? new();
            }

            var result = action(this.ReservedTweets);

            var json = JsonSerializer.Serialize(this.ReservedTweets);
            await File.WriteAllTextAsync(storagePath, json);

            return result;
        }
        finally { this.Lock.Release(); }
    }

    public async ValueTask AddAsync(string twitterUserId, string textToTweet)
    {
        await this.ActionAsync(reservedTweets =>
        {
            var tweets = default(List<ReservedTweet>);
            if (!reservedTweets.TryGetValue(twitterUserId, out tweets))
            {
                tweets = new List<ReservedTweet>();
                reservedTweets.Add(twitterUserId, tweets);
            }

            var newTweet = new ReservedTweet
            {
                Id = Guid.NewGuid().ToString(),
                TextToTweet = textToTweet,
                Order = tweets.DefaultIfEmpty().Max(t => t?.Order ?? 0) + 1
            };
            tweets.Add(newTweet);

            return newTweet.Id;
        });
    }

    public async ValueTask<ReservedTweet[]> GetAllAsync(string twitterUserId)
    {
        return await this.ActionAsync(reservedTweets =>
        {
            if (reservedTweets.TryGetValue(twitterUserId, out var tweets))
                return tweets.ToArray();
            return null;
        }) ?? Array.Empty<ReservedTweet>();
    }

    public ValueTask<ReservedTweet?> GetAsync(string twitterUserId, string id)
    {
        return this.ActionAsync(reservedTweets =>
        {
            if (!reservedTweets.TryGetValue(twitterUserId, out var tweets)) return null;
            return tweets.FirstOrDefault(t => t.Id == id);
        });
    }

    public async ValueTask UpdateAsync(string twitterUserId, string id, string textToTweet, int order, bool isTweeted)
    {
        await this.ActionAsync<object>(reservedTweets =>
        {
            if (!reservedTweets.TryGetValue(twitterUserId, out var tweets)) return null;
            var targetTweet = tweets.FirstOrDefault(t => t.Id == id);
            if (targetTweet != null)
            {
                targetTweet.TextToTweet = textToTweet;
                targetTweet.Order = order;
                targetTweet.IsTweeted = isTweeted;
            }
            return null;
        });
    }

    public async ValueTask DeleteAsync(string twitterUserId, string id)
    {
        await this.ActionAsync<object>(reservedTweets =>
        {
            if (reservedTweets.TryGetValue(twitterUserId, out var tweets))
                tweets.RemoveAll(t => t.Id == id);
            return null;
        });
    }
}
