/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />

class Tweet {
    Id: number;
    OwnerUserId: string;
    TextToTweet: string;
    Order: number;
    IsTweeted: boolean;
}

interface IScope extends ng.IScope {
    tweets: Tweet[];
}

var app = angular.module('app', ['ngResource']);

app.controller('editorController', ($scope: IScope, $resource: any) => {
    $scope.tweets = [];
    console.dir($resource);
});