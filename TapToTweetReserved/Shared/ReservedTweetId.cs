namespace TapToTweetReserved.Shared
{
    public partial class ReservedTweetId
    {
        public ReservedTweetId(string id)
        {
            this.OnConstruction();
            this.Id = id;
        }
    }
}
