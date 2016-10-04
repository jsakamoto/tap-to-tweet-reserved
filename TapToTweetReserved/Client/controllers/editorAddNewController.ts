class EditorAddNewController extends EditorEditControllerBase {
    reservedTweets: IReservedTweets;

    constructor($scope: IEditScope, $location: ng.ILocationService, reservedTweets: IReservedTweets) {
        super($scope, $location);
        this.reservedTweets = reservedTweets;
        this.$scope.tweet = reservedTweets.createNew({ TextToTweet: '' });
        this.watchCharCount();
    }

    public ok() {
        this.$scope.tweet.$save().then(() => {
            this.reservedTweets.push(this.$scope.tweet);
            this.goBack();
        });
    }
}

app.controller('editorAddNewController', ['$scope', '$location', 'reservedTweets', EditorAddNewController]);
