interface Tweet extends ngres.IResource<Tweet> {
    Id: number;
    TextToTweet: string;
    Order: number;
    IsTweeted: boolean;
    selected: boolean;
}
