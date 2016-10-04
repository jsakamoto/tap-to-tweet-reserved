interface IReservedTweets extends ngres.IResourceArray<Tweet> {
    createNew(data: Object): Tweet;
}
