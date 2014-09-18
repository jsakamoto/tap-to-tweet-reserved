/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/angularjs/angular-resource.d.ts" />
var Tweet = (function () {
    function Tweet() {
    }
    return Tweet;
})();

var app = angular.module('app', ['ngResource']);

app.config(function ($httpProvider) {
    // Anti IE cache
    if (!$httpProvider.defaults.headers.get)
        $httpProvider.defaults.headers.get = {};
    $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';
});

app.controller('editorController', function ($scope, $resource) {
    var reservedTweets = $resource('/api/ReservedTweets/:id', { id: '@Id' });
    $scope.tweets = reservedTweets.query();

    // Select
    $scope.selectTweet = function (tweet) {
        tweet.selected = !(tweet.selected || false);
        var selecteds = $scope.tweets.filter(function (t) {
            return t.selected == true;
        });
        $scope.selectedAny = selecteds.length > 0;
        $scope.selectedAnyTweeted = selecteds.some(function (t) {
            return t.IsTweeted == true;
        });
    };

    // Edit
    $scope.editTweet = function () {
        var selecteds = $scope.tweets.filter(function (t) {
            return t.selected == true;
        });
        if (selecteds.length == 0)
            return;
        window.location.href = "Editor/Edit/" + selecteds[0].Id.toString();
    };

    // Delete
    $scope.deleteTweet = function () {
        $scope.tweets.filter(function (t) {
            return t.selected == true;
        }).forEach(function (t) {
            return t.$remove();
        });
        $scope.tweets = $scope.tweets.filter(function (t) {
            return (t.selected || false) == false;
        });
    };

    // Move Up/Down
    var moveUpOrDownTweet = function (direction) {
        var limit = ($scope.tweets.length - 1) * ((1 + direction) / 2);
        var selecteds = $scope.tweets.filter(function (t) {
            return t.selected == true;
        }).sort(function (a, b) {
            return -1 * direction * (a.Order - b.Order);
        });
        $.each(selecteds, function (n, t) {
            var index = $scope.tweets.indexOf(t);
            if (index == limit)
                return false;
            $scope.tweets.splice(index, 1);
            $scope.tweets.splice(index + direction, 0, t);
        });
        $.each($scope.tweets, function (n, t) {
            if (t.Order != n + 1) {
                t.Order = n + 1;
                t.$save();
            }
        });
    };

    $scope.moveUpTweet = function () {
        return moveUpOrDownTweet(-1);
    };
    $scope.moveDownTweet = function () {
        return moveUpOrDownTweet(+1);
    };

    // Reload
    $scope.reloadTweet = function () {
        var selecteds = $scope.tweets.filter(function (t) {
            return t.selected == true;
        });
        selecteds.filter(function (t) {
            return t.IsTweeted == true;
        }).forEach(function (t) {
            t.IsTweeted = false;
            t.$save();
        });
        $scope.selectedAnyTweeted = false;
    };
});
//# sourceMappingURL=Editor.js.map
