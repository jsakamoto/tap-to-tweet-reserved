
class EditorEditControllerBase {

    public tweet: Tweet;

    public charCount: number;

    public overflow: boolean;

    constructor(
        private $scope: ng.IScope,
        private $location: ng.ILocationService,
        tweet: Tweet
    ) {
        this.tweet = tweet;
        this.watchCharCount();
    }

    private watchCharCount() {
        this.$scope.$watch(() => this.tweet.TextToTweet, () => {
            const MAXCHARS = 140;
            this.charCount = charCounter(this.tweet.TextToTweet || '', MAXCHARS);
            this.overflow = this.charCount < 0;
        });
    }

    public goBack() {
        this.$location.url('/');
    }
}
