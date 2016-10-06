var EditorHomeController = (function () {
    function EditorHomeController($scope, $location, reservedTweets) {
        var _this = this;
        this.$scope = $scope;
        this.$location = $location;
        this.$scope.loaded = false;
        $scope.tweets = reservedTweets;
        $scope.tweets.$promise.then(function () {
            $scope.loaded = true;
            _this.updateState();
        });
    }
    EditorHomeController.prototype.updateState = function () {
        var selecteds = this.$scope.tweets.filter(function (t) { return t.selected == true; });
        this.$scope.selectedAny = selecteds.length > 0;
        this.$scope.selectedAnyTweeted = selecteds.some(function (t) { return t.IsTweeted == true; });
    };
    EditorHomeController.prototype.selectTweet = function (tweet) {
        tweet.selected = !(tweet.selected || false);
        this.updateState();
    };
    EditorHomeController.prototype.addNewTweet = function () {
        this.$location.url('/addnew');
    };
    EditorHomeController.prototype.editTweet = function () {
        var selecteds = this.$scope.tweets.filter(function (t) { return t.selected == true; });
        if (selecteds.length == 0)
            return;
        this.$location.url('/edit/' + selecteds[0].Id.toString());
    };
    EditorHomeController.prototype.deleteTweet = function () {
        var _this = this;
        if (confirm('Delete reserved tweet.\nSure?') == false)
            return;
        var selecteds = this.$scope.tweets.filter(function (t) { return t.selected == true; });
        selecteds.forEach(function (t) {
            t.$remove();
            var index = _this.$scope.tweets.indexOf(t);
            _this.$scope.tweets.splice(index, 1);
        });
        this.updateState();
    };
    EditorHomeController.prototype.reloadTweet = function () {
        if (confirm('Reload the tweeted tweet.\nSure?') == false)
            return;
        var selecteds = this.$scope.tweets.filter(function (t) { return t.selected == true; });
        selecteds.filter(function (t) { return t.IsTweeted == true; })
            .forEach(function (t) {
            t.IsTweeted = false;
            t.$save();
        });
        this.updateState();
    };
    EditorHomeController.prototype.moveUpTweet = function () { this.moveUpOrDownTweet(-1); };
    EditorHomeController.prototype.moveDownTweet = function () { this.moveUpOrDownTweet(+1); };
    EditorHomeController.prototype.moveUpOrDownTweet = function (direction) {
        var _this = this;
        var limit = (this.$scope.tweets.length - 1) * ((1 + direction) / 2);
        var selecteds = this.$scope.tweets
            .filter(function (t) { return t.selected == true; })
            .sort(function (a, b) { return -1 * direction * (a.Order - b.Order); });
        $.each(selecteds, function (n, t) {
            var index = _this.$scope.tweets.indexOf(t);
            if (index == limit)
                return false;
            _this.$scope.tweets.splice(index, 1);
            _this.$scope.tweets.splice(index + direction, 0, t);
        });
        $.each(this.$scope.tweets, function (n, t) {
            if (t.Order != n + 1) {
                t.Order = n + 1;
                t.$save();
            }
        });
    };
    return EditorHomeController;
}());
app.controller('editorHomeController', ['$scope', '$location', 'reservedTweets', EditorHomeController]);
//# sourceMappingURL=editorHomeController.js.map