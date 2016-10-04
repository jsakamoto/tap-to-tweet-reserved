interface IHomeScope extends ng.IScope {
    tweets: Tweet[];
    loaded: boolean;
    selectedAny: boolean;
    selectedAnyTweeted: boolean;
}

class EditorHomeController {
    $scope: IHomeScope;
    $location: ng.ILocationService;

    constructor($scope: IHomeScope, $location: ng.ILocationService, reservedTweets: IReservedTweets) {

        this.$scope = $scope;
        this.$location = $location;
        this.$scope.loaded = false;

        $scope.tweets = reservedTweets;
        $scope.tweets.$promise.then(() => {
            $scope.loaded = true;
            this.updateState();
        });
    }

    private updateState() {
        var selecteds = this.$scope.tweets.filter(t => t.selected == true);
        this.$scope.selectedAny = selecteds.length > 0;
        this.$scope.selectedAnyTweeted = selecteds.some(t => t.IsTweeted == true);
    }

    // Select
    public selectTweet(tweet: Tweet) {
        tweet.selected = !(tweet.selected || false);
        this.updateState();
    }

    // AddNew
    public addNewTweet() {
        this.$location.url('/addnew');
    }

    // Edit
    public editTweet() {
        var selecteds = this.$scope.tweets.filter(t => t.selected == true);
        if (selecteds.length == 0) return;
        this.$location.url('/edit/' + selecteds[0].Id.toString());
    }

    // Delete
    public deleteTweet() {
        if (confirm('Delete reserved tweet.\nSure?') == false) return;
        var selecteds = this.$scope.tweets.filter(t => t.selected == true);
        selecteds.forEach(t => {
            t.$remove();
            var index = this.$scope.tweets.indexOf(t);
            this.$scope.tweets.splice(index, 1);
        });
        this.updateState();
    }

    // Reload
    public reloadTweet() {
        if (confirm('Reload the tweeted tweet.\nSure?') == false) return;
        var selecteds = this.$scope.tweets.filter(t => t.selected == true);
        selecteds.filter(t => t.IsTweeted == true)
            .forEach(t => {
                t.IsTweeted = false;
                t.$save();
            });
        this.updateState();
    }

    // Move Up
    public moveUpTweet() { this.moveUpOrDownTweet(-1); }

    // Move Down
    public moveDownTweet() { this.moveUpOrDownTweet(+1); }

    private moveUpOrDownTweet(direction: number) {
        var limit = (this.$scope.tweets.length - 1) * ((1 + direction) / 2);
        var selecteds = this.$scope.tweets
            .filter(t => t.selected == true)
            .sort((a, b) => -1 * direction * (a.Order - b.Order));
        $.each(selecteds, (n, t) => {
            var index = this.$scope.tweets.indexOf(t);
            if (index == limit) return false;
            this.$scope.tweets.splice(index, 1);
            this.$scope.tweets.splice(index + direction, 0, t);
        });
        $.each(this.$scope.tweets, (n, t) => {
            if (t.Order != n + 1) {
                t.Order = n + 1;
                t.$save();
            }
        });
    }
}

app.controller('editorHomeController', ['$scope', '$location', 'reservedTweets', EditorHomeController]);
