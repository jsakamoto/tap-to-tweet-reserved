using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace TapToTweetReserved.Models
{
    public class ReservedTweet
    {
        public int Id { get; set; }

        [JsonIgnore]
        public string OwnerUserId { get; set; }

        [Required, AllowHtml]
        public string TextToTweet { get; set; }

        public int Order { get; set; }

        public bool IsTweeted { get; set; }
    }
}