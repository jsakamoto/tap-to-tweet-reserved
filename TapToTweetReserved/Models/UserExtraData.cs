using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TapToTweetReserved.Models
{
    public class UserExtraData
    {
        public string UserId { get; set; }

        public string AccessToken { get; set; }

        public string AccessSecret { get; set; }
    }
}