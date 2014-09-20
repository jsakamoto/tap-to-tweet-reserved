/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/angularjs/angular-resource.d.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var app = angular.module('app', ['ngResource', 'ngRoute']);

app.config(function ($httpProvider, $routeProvider) {
    // Anti IE cache
    if (!$httpProvider.defaults.headers.get)
        $httpProvider.defaults.headers.get = {};
    $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';

    // Setup routes.
    $routeProvider.when('/', {
        title: 'Edit Reserved Tweet',
        controller: 'editorHomeController', controllerAs: 'ctrl',
        templateUrl: '/views/editor/homeView.html'
    }).when('/addnew', {
        title: 'Add New Tweet',
        controller: 'editorAddNewController', controllerAs: 'ctrl',
        templateUrl: '/views/editor/editView.html'
    }).when('/edit/:id', {
        title: 'Edit the Tweet',
        controller: 'editorEditController', controllerAs: 'ctrl',
        templateUrl: '/views/editor/editView.html'
    });
});

app.run(function ($rootScope) {
    $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
        $rootScope.title = current.$$route.title;
    });
});

app.service('reservedTweets', function ($resource) {
    return $resource('/api/ReservedTweets/:id', { id: '@Id' });
});

app.filter('htmlLineBreak', function ($injector) {
    var domElem = null;
    var sce = null;
    return function (input) {
        if (input == null)
            return null;
        sce = sce || $injector.get('$sce');
        domElem = domElem || $(window.document.createElement('div'));
        return sce.trustAsHtml(domElem.text(input).html().replace(/(\n)|(\r\n)|(\r)/ig, '<br/>').replace(/ /ig, '&nbsp;'));
    };
});

var EditorHomeController = (function () {
    function EditorHomeController($scope, reservedTweets, $location) {
        this.$scope = $scope;
        this.$location = $location;
        this.$scope.loaded = false;

        $scope.tweets = reservedTweets.query();
        $scope.tweets.$promise.then(function () {
            $scope.loaded = true;
        });
    }
    // Select
    EditorHomeController.prototype.selectTweet = function (tweet) {
        tweet.selected = !(tweet.selected || false);
        var selecteds = this.$scope.tweets.filter(function (t) {
            return t.selected == true;
        });
        this.$scope.selectedAny = selecteds.length > 0;
        this.$scope.selectedAnyTweeted = selecteds.some(function (t) {
            return t.IsTweeted == true;
        });
    };

    // AddNew
    EditorHomeController.prototype.addNewTweet = function () {
        this.$location.url('/addnew');
    };

    // Edit
    EditorHomeController.prototype.editTweet = function () {
        var selecteds = this.$scope.tweets.filter(function (t) {
            return t.selected == true;
        });
        if (selecteds.length == 0)
            return;
        this.$location.url('/edit/' + selecteds[0].Id.toString());
    };

    // Delete
    EditorHomeController.prototype.deleteTweet = function () {
        if (confirm('Delete reserved tweet.\nSure?') == false)
            return;
        this.$scope.tweets.filter(function (t) {
            return t.selected == true;
        }).forEach(function (t) {
            return t.$remove();
        });
        this.$scope.tweets = this.$scope.tweets.filter(function (t) {
            return (t.selected || false) == false;
        });
    };

    // Reload
    EditorHomeController.prototype.reloadTweet = function () {
        if (confirm('Reload the tweeted tweet.\nSure?') == false)
            return;
        var selecteds = this.$scope.tweets.filter(function (t) {
            return t.selected == true;
        });
        selecteds.filter(function (t) {
            return t.IsTweeted == true;
        }).forEach(function (t) {
            t.IsTweeted = false;
            t.$save();
        });
        this.$scope.selectedAnyTweeted = false;
    };

    // Move Up
    EditorHomeController.prototype.moveUpTweet = function () {
        this.moveUpOrDownTweet(-1);
    };

    // Move Down
    EditorHomeController.prototype.moveDownTweet = function () {
        this.moveUpOrDownTweet(+1);
    };

    EditorHomeController.prototype.moveUpOrDownTweet = function (direction) {
        var _this = this;
        var limit = (this.$scope.tweets.length - 1) * ((1 + direction) / 2);
        var selecteds = this.$scope.tweets.filter(function (t) {
            return t.selected == true;
        }).sort(function (a, b) {
            return -1 * direction * (a.Order - b.Order);
        });
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
})();

var EditorEditControllerBase = (function () {
    function EditorEditControllerBase($scope, $location) {
        this.$scope = $scope;
        this.$location = $location;
    }
    EditorEditControllerBase.prototype.watchCharCount = function () {
        var _this = this;
        this.$scope.$watch('tweet.TextToTweet', function () {
            var MAXCHARS = 140;
            _this.$scope.charCount = MAXCHARS - (_this.$scope.tweet.TextToTweet || '').length;
            _this.$scope.overflow = _this.$scope.charCount <= 0;
        });
    };

    EditorEditControllerBase.prototype.goBack = function () {
        this.$location.url('/');
    };
    return EditorEditControllerBase;
})();

var EditorEditController = (function (_super) {
    __extends(EditorEditController, _super);
    function EditorEditController($scope, $location, reservedTweets, $routeParams) {
        _super.call(this, $scope, $location);
        this.$scope.tweet = reservedTweets.get({ id: $routeParams.id });
        this.watchCharCount();
    }
    EditorEditController.prototype.ok = function () {
        var _this = this;
        this.$scope.tweet.$save().then(function () {
            return _this.goBack();
        });
    };
    return EditorEditController;
})(EditorEditControllerBase);

var EditorAddNewController = (function (_super) {
    __extends(EditorAddNewController, _super);
    function EditorAddNewController($scope, $location, reservedTweets) {
        _super.call(this, $scope, $location);
        this.reservedTweets = reservedTweets;
        this.$scope.tweet = { TextToTweet: '' };
        this.watchCharCount();
    }
    EditorAddNewController.prototype.ok = function () {
        var _this = this;
        this.reservedTweets.save(this.$scope.tweet, function () {
            return _this.goBack();
        });
    };
    return EditorAddNewController;
})(EditorEditControllerBase);

app.controller('editorHomeController', EditorHomeController).controller('editorAddNewController', EditorAddNewController).controller('editorEditController', EditorEditController);
//# sourceMappingURL=Editor.js.map
