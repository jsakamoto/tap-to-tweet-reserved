class EditorEditController extends EditorEditControllerBase {
    constructor(
        $scope: ng.IScope,
        $location: ng.ILocationService,
        reservedTweets: IReservedTweets,
        $routeParams: any
    ) {
        super($scope, $location, reservedTweets.filter(t => t.Id == $routeParams.id)[0]);
    }

    public ok() {
        this.tweet.$save()
            .then(() => this.goBack());
    }
}

app.controller('editorEditController', ['$scope', '$location', 'reservedTweets', '$routeParams', EditorEditController]);
