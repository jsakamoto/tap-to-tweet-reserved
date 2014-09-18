/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
var Tweet = (function () {
    function Tweet() {
    }
    return Tweet;
})();

var app = angular.module('app', ['ngResource']);

app.controller('editorController', function ($scope, $resource) {
    $scope.tweets = [];
    console.dir($resource);
});
//# sourceMappingURL=Editor.js.map
