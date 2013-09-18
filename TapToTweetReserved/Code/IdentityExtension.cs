using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using TapToTweetReserved.Models;
using Newtonsoft.Json;

namespace TapToTweetReserved
{
    public static class IdentityExtension
    {
        public static T ExtraData<T>(this IPrincipal principal)
        {
            return
                principal.Identity.IsAuthenticated ?
                JsonConvert.DeserializeObject<T>((principal.Identity as FormsIdentity).Ticket.UserData) :
                default(T);
        }
    }
}