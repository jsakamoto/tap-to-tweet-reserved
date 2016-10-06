class EditorEditController extends EditorEditControllerBase {
    constructor($scope: IEditScope, $location: ng.ILocationService, reservedTweets: IReservedTweets, $routeParams: any) {
        super($scope, $location);
        this.$scope.tweet = reservedTweets.filter(t => t.Id == $routeParams.id)[0];
        this.watchCharCount();
    }

    public ok() {
        this.$scope.tweet.$save()
            .then(() => this.goBack());
    }
}

app.controller('editorEditController', ['$scope', '$location', 'reservedTweets', '$routeParams', EditorEditController]);
