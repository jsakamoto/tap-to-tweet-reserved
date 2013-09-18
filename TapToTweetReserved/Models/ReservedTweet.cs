using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hammock.Attributes.Validation;

namespace TapToTweetReserved.Models
{
    public class ReservedTweet
    {
        public int Id { get; set; }

        public string OwnerUserId { get; set; }

        [Required]
        public string TextToTweet { get; set; }

        public int Order { get; set; }

        public bool IsTweeted { get; set; }
    }
}