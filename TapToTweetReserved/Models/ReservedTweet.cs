using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hammock.Attributes.Validation;

namespace TapToTweetReserved.Models
{
    public class ReservedTweet
    {
        public int Id { get; set; }

        public string OwnerUserId { get; set; }

        [Required, AllowHtml]
        public string TextToTweet { get; set; }

        public int Order { get; set; }

        public bool IsTweeted { get; set; }
    }
}