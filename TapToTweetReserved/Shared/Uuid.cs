using System;

namespace TapToTweetReserved.Shared
{
    public partial class Uuid
    {
        public static Uuid NewUuid()
        {
            return new Uuid { Value = Guid.NewGuid().ToString() };
        }
    }
}
