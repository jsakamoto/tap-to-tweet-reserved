using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TapToTweetReserved.Code
{
    public static class JsonAppSettings
    {
        public static Dictionary<string, string> AsDictionary(string key)
        {
            var appSetting = ConfigurationManager.AppSettings[key];
            if (appSetting == null) return null;
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(appSetting);
        }
    }
}