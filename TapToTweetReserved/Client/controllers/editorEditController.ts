class EditorEditController extends EditorEditControllerBase {
    constructor(
        $scope: ng.IScope,
        $location: ng.ILocationService,
        private reservedTweets: IReservedTweets,
        private $routeParams: any
    ) {
        super($scope, $location, angular.copy(reservedTweets.filter(t => t.Id == $routeParams.id)[0]));
    }

    public ok() {
        let editTarget = this.reservedTweets.filter(t => t.Id == this.$routeParams.id)[0];
        angular.copy(this.tweet, editTarget);
        editTarget.$save()
            .then(() => this.goBack());
    }
}

app.controller('editorEditController', ['$scope', '$location', 'reservedTweets', '$routeParams', EditorEditController]);
