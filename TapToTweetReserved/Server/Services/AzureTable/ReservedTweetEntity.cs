using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TapToTweetReserved.Shared;

namespace TapToTweetReserved.Server.Services.AzureTable
{
    public class ReservedTweetEntity : Microsoft.Azure.Cosmos.Table.TableEntityAdapter<ReservedTweet>
    {

    }
}
