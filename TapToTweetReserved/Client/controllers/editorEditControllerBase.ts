interface IEditScope extends ng.IScope {
    tweet: Tweet;
    charCount: number;
    overflow: boolean;
}

class EditorEditControllerBase {
    $scope: IEditScope;
    $location: ng.ILocationService;
    constructor($scope: IEditScope, $location: ng.ILocationService) {
        this.$scope = $scope;
        this.$location = $location;
    }

    public watchCharCount() {
        this.$scope.$watch('tweet.TextToTweet', () => {
            const MAXCHARS = 140;
            this.$scope.charCount = charCounter(this.$scope.tweet.TextToTweet || '', MAXCHARS);
            this.$scope.overflow = this.$scope.charCount <= 0;
        });
    }

    public goBack() {
        this.$location.url('/');
    }
}
