var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var app = angular.module('app', ['ngResource', 'ngRoute']);
app.config(function ($httpProvider, $routeProvider) {
    if (!$httpProvider.defaults.headers.get)
        $httpProvider.defaults.headers.get = {};
    $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';
    $routeProvider
        .when('/', {
        title: 'Edit Reserved Tweet',
        controller: 'editorHomeController', controllerAs: 'ctrl',
        templateUrl: '/views/editor/homeView.html'
    })
        .when('/addnew', {
        title: 'Add New Tweet',
        controller: 'editorAddNewController', controllerAs: 'ctrl',
        templateUrl: '/views/editor/editView.html'
    })
        .when('/edit/:id', {
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
    var resclass = $resource('/api/ReservedTweets/:id', { id: '@Id' });
    var resarray = resclass.query();
    resarray.createNew = function (data) { return new resclass(data); };
    return resarray;
});
app.filter('htmlLineBreak', function ($injector) {
    var domElem = null;
    var sce = null;
    return function (input) {
        if (input == null)
            return null;
        sce = sce || $injector.get('$sce');
        domElem = domElem || $(window.document.createElement('div'));
        return sce.trustAsHtml(domElem.text(input).html()
            .replace(/(\n)|(\r\n)|(\r)/ig, '<br/>')
            .replace(/ /ig, '&nbsp;'));
    };
});
var EditorHomeController = (function () {
    function EditorHomeController($scope, reservedTweets, $location) {
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
        this.$scope.tweet = reservedTweets.filter(function (t) { return t.Id == $routeParams.id; })[0];
        this.watchCharCount();
    }
    EditorEditController.prototype.ok = function () {
        var _this = this;
        this.$scope.tweet.$save()
            .then(function () { return _this.goBack(); });
    };
    return EditorEditController;
})(EditorEditControllerBase);
var EditorAddNewController = (function (_super) {
    __extends(EditorAddNewController, _super);
    function EditorAddNewController($scope, $location, reservedTweets) {
        _super.call(this, $scope, $location);
        this.reservedTweets = reservedTweets;
        this.$scope.tweet = reservedTweets.createNew({ TextToTweet: '' });
        this.watchCharCount();
    }
    EditorAddNewController.prototype.ok = function () {
        var _this = this;
        this.$scope.tweet.$save().then(function () {
            _this.reservedTweets.push(_this.$scope.tweet);
            _this.goBack();
        });
    };
    return EditorAddNewController;
})(EditorEditControllerBase);
app.controller('editorHomeController', EditorHomeController)
    .controller('editorAddNewController', EditorAddNewController)
    .controller('editorEditController', EditorEditController);
//# sourceMappingURL=Editor.js.map