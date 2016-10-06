class EditorAddNewController extends EditorEditControllerBase {

    constructor(
        $scope: ng.IScope,
        $location: ng.ILocationService,
        private reservedTweets: IReservedTweets
    ) {
        super($scope, $location, reservedTweets.createNew({ TextToTweet: '' }));
    }

    public ok() {
        this.tweet.$save().then(() => {
            this.reservedTweets.push(this.tweet);
            this.goBack();
        });
    }
}

app.controller('editorAddNewController', ['$scope', '$location', 'reservedTweets', EditorAddNewController]);
