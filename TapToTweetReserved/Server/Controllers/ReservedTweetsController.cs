﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TapToTweetReserved.Server.Services;
using TapToTweetReserved.Shared;
using System.Linq;

namespace TapToTweetReserved.Server.Controllers
{
    [Authorize]
    public class ReservedTweetsController : Controller
    {
        private TwitterConfiguration TwitterConfiguration { get; }

        private IReservedTweetsRepository ReservedTweetsRepository { get; }

        public ReservedTweetsController(
            TwitterConfiguration twitterConfiguration,
            IReservedTweetsRepository reservedTweetsRepository)
        {
            TwitterConfiguration = twitterConfiguration;
            ReservedTweetsRepository = reservedTweetsRepository;
        }

        [HttpPost("/api/reservedtweets")]
        public async Task<Guid> PostAsync([FromBody]ReservedTweet newTweet)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            var newTweetId = await this.ReservedTweetsRepository.AddAsync(twitterUserId, newTweet.TextToTweet);
            return newTweetId;
        }

        [HttpGet("/api/reservedtweets")]
        public async Task<ReservedTweet[]> GetAllAsync()
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            var reservedTweets = await this.ReservedTweetsRepository.GetAllAsync(twitterUserId);
            return reservedTweets.OrderBy(t => t.Id).ToArray();
        }

        [HttpGet("/api/reservedtweets/{id}")]
        public async Task<ReservedTweet> GetAsync(Guid id)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            var reservedTweet = await this.ReservedTweetsRepository.GetAsync(twitterUserId, id);
            return reservedTweet;
        }

        [HttpPut("/api/reservedtweets/{id}")]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody]ReservedTweet tweet)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            await this.ReservedTweetsRepository.UpdateAsync(twitterUserId, id,
                tweet.TextToTweet,
                tweet.Order,
                tweet.IsTweeted);
            return Ok();
        }

        [HttpDelete("/api/reservedtweets/{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            await this.ReservedTweetsRepository.DeleteAsync(twitterUserId, id);
            return Ok();
        }

        [HttpPost("/api/reservedtweets/{id}/tweet")]
        public async Task<IActionResult> PostTweetAsync(Guid id)
        {
            var twitterUserId = this.User.Claims.GetTwitterUserId();
            var reservedTweet = await this.ReservedTweetsRepository.GetAsync(twitterUserId, id);
            if (reservedTweet == null) return NotFound();

            var token = CoreTweet.Tokens.Create(
                TwitterConfiguration.ConsumerAPIKey,
                TwitterConfiguration.ConsumerSecret,
                this.User.Claims.GetTwitterAccessToken(),
                this.User.Claims.GetTwitterAccessTokenSecret());
            await token.Statuses.UpdateAsync(reservedTweet.TextToTweet);

            return Ok();
        }
    }
}
