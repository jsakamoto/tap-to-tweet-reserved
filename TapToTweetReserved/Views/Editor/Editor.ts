/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/angularjs/angular-resource.d.ts" />

class Tweet {
    Id: number;
    OwnerUserId: string;
    TextToTweet: string;
    Order: number;
    IsTweeted: boolean;
}

interface IScope extends ng.IScope {
    tweets: any[];

    selectedAny: boolean;
    selectedAnyTweeted: boolean;

    selectTweet: (a: any) => void;
    editTweet: (a: any) => void;
    deleteTweet: () => void;
    moveUpTweet: () => void;
    moveDownTweet: () => void;
    reloadTweet: () => void;
}

var app = angular.module('app', ['ngResource']);

app.config(($httpProvider: ng.IHttpProvider) => {
    // Anti IE cache
    if (!$httpProvider.defaults.headers.get) $httpProvider.defaults.headers.get = {};
    $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';
});

app.controller('editorController', ($scope: IScope, $resource: ng.resource.IResourceService) => {
    var reservedTweets = $resource('/api/ReservedTweets/:id', { id: '@Id' });
    $scope.tweets = reservedTweets.query();

    // Select
    $scope.selectTweet = tweet => {
        tweet.selected = !(tweet.selected || false);
        var selecteds = $scope.tweets.filter(t => t.selected == true);
        $scope.selectedAny = selecteds.length > 0;
        $scope.selectedAnyTweeted = selecteds.some(t => t.IsTweeted == true);
    };

    // Edit
    $scope.editTweet = () => {
        var selecteds = $scope.tweets.filter(t => t.selected == true);
        if (selecteds.length == 0) return;
        window.location.href = "Editor/Edit/" + selecteds[0].Id.toString();
    };

    // Delete
    $scope.deleteTweet = () => {
        $scope.tweets
            .filter(t => t.selected == true)
            .forEach(t => t.$remove());
        $scope.tweets = $scope.tweets.filter(t => (t.selected || false) == false);
    };

    // Move Up/Down
    var moveUpOrDownTweet = (direction: number) => {
        var limit = ($scope.tweets.length - 1) * ((1 + direction) / 2);
        var selecteds = $scope.tweets
            .filter(t => t.selected == true)
            .sort((a, b) => -1 * direction * (a.Order - b.Order));
        $.each(selecteds, (n, t) => {
            var index = $scope.tweets.indexOf(t);
            if (index == limit) return false;
            $scope.tweets.splice(index, 1);
            $scope.tweets.splice(index + direction, 0, t);
        });
        $.each($scope.tweets, (n, t) => {
            if (t.Order != n + 1) {
                t.Order = n + 1;
                t.$save();
            }
        });
    };

    $scope.moveUpTweet = () => moveUpOrDownTweet(-1);
    $scope.moveDownTweet = () => moveUpOrDownTweet(+1);

    // Reload
    $scope.reloadTweet = () => {
        var selecteds = $scope.tweets.filter(t => t.selected == true);
        selecteds.filter(t => t.IsTweeted == true)
            .forEach(t => {
                t.IsTweeted = false;
                t.$save();
            });
        $scope.selectedAnyTweeted = false;
    }
}); 