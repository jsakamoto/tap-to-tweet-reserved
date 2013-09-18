using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TapToTweetReserved.Models
{
    public class TapToTweetReservedDb : DbContext
    {
        public DbSet<ReservedTweet> ReservedTweets { get; set; }
    }
}