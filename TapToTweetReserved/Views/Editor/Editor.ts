/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../scripts/typings/angularjs/angular-resource.d.ts" />

import ngres = ng.resource;
var app = angular.module('app', ['ngResource', 'ngRoute']);


interface Tweet extends ngres.IResource<Tweet> {
    Id: number;
    TextToTweet: string;
    Order: number;
    IsTweeted: boolean;
    selected: boolean;
}


app.config(($httpProvider: ng.IHttpProvider, $routeProvider: ng.route.IRouteProvider) => {
    // Anti IE cache
    if (!$httpProvider.defaults.headers.get) $httpProvider.defaults.headers.get = {};
    $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';

    // Setup routes.
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
        })
    ;
});

app.run($rootScope => {
    $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
        $rootScope.title = current.$$route.title;
    });
});

interface IReservedTweets extends ngres.IResourceClass<Tweet> { }

app.service('reservedTweets', ($resource: ngres.IResourceService) => {
    return $resource<Tweet>('/api/ReservedTweets/:id', { id: '@Id' });
});

app.filter('htmlLineBreak', ($injector) => {
    var domElem: JQuery = null;
    var sce = null;
    return (input: string) => {
        if (input == null) return null;
        sce = sce || $injector.get('$sce');
        domElem = domElem || $(window.document.createElement('div'));
        return sce.trustAsHtml(
            domElem.text(input).html()
                .replace(/(\n)|(\r\n)|(\r)/ig, '<br/>')
                .replace(/ /ig, '&nbsp;'));
    };
});

interface IHomeScope extends ng.IScope {
    tweets: Tweet[];
    loaded: boolean;
    selectedAny: boolean;
    selectedAnyTweeted: boolean;
}

class EditorHomeController {
    $scope: IHomeScope;
    $location: ng.ILocationService;

    constructor($scope: IHomeScope, reservedTweets: IReservedTweets, $location: ng.ILocationService) {

        this.$scope = $scope;
        this.$location = $location;
        this.$scope.loaded = false;

        $scope.tweets = reservedTweets.query();
        $scope.tweets.$promise.then(() => { $scope.loaded = true; });
    }

    // Select
    public selectTweet(tweet: Tweet) {
        tweet.selected = !(tweet.selected || false);
        var selecteds = this.$scope.tweets.filter(t => t.selected == true);
        this.$scope.selectedAny = selecteds.length > 0;
        this.$scope.selectedAnyTweeted = selecteds.some(t => t.IsTweeted == true);
    }

    // AddNew
    public addNewTweet() {
        this.$location.url('/addnew');
    }

    // Edit
    public editTweet() {
        var selecteds = this.$scope.tweets.filter(t => t.selected == true);
        if (selecteds.length == 0) return;
        this.$location.url('/edit/' + selecteds[0].Id.toString());
    }

    // Delete
    public deleteTweet() {
        this.$scope.tweets
            .filter(t => t.selected == true)
            .forEach(t => t.$remove());
        this.$scope.tweets = this.$scope.tweets.filter(t => (t.selected || false) == false);
    }

    // Reload
    public reloadTweet() {
        var selecteds = this.$scope.tweets.filter(t => t.selected == true);
        selecteds.filter(t => t.IsTweeted == true)
            .forEach(t => {
                t.IsTweeted = false;
                t.$save();
            });
        this.$scope.selectedAnyTweeted = false;
    }

    // Move Up
    public moveUpTweet() { this.moveUpOrDownTweet(-1); }

    // Move Down
    public moveDownTweet() { this.moveUpOrDownTweet(+1); }

    private moveUpOrDownTweet(direction: number) {
        var limit = (this.$scope.tweets.length - 1) * ((1 + direction) / 2);
        var selecteds = this.$scope.tweets
            .filter(t => t.selected == true)
            .sort((a, b) => -1 * direction * (a.Order - b.Order));
        $.each(selecteds, (n, t) => {
            var index = this.$scope.tweets.indexOf(t);
            if (index == limit) return false;
            this.$scope.tweets.splice(index, 1);
            this.$scope.tweets.splice(index + direction, 0, t);
        });
        $.each(this.$scope.tweets, (n, t) => {
            if (t.Order != n + 1) {
                t.Order = n + 1;
                t.$save();
            }
        });
    }
}

interface IEditScope extends ng.IScope {
    tweet: Tweet;
    charCount: number;
    overflow: boolean;
}

class EditorEditControllerBase {
    $scope: IEditScope;
    $location: ng.ILocationService;
    constructor($scope: IEditScope, $location: ng.ILocationService) {
        this.$scope = $scope;
        this.$location = $location;
    }

    public watchCharCount() {
        this.$scope.$watch('tweet.TextToTweet', () => {
            var MAXCHARS = 140;
            this.$scope.charCount = MAXCHARS - (this.$scope.tweet.TextToTweet || '').length;
            this.$scope.overflow = this.$scope.charCount <= 0;
        });
    }

    public goBack() {
        this.$location.url('/');
    }
}

class EditorEditController extends EditorEditControllerBase {
    constructor($scope: IEditScope, $location: ng.ILocationService, reservedTweets: IReservedTweets, $routeParams: any) {
        super($scope, $location);
        this.$scope.tweet = reservedTweets.get({ id: $routeParams.id });
        this.watchCharCount();
    }

    public ok() {
        this.$scope.tweet.$save()
            .then(() => this.goBack());
    }
}

class EditorAddNewController extends EditorEditControllerBase {
    reservedTweets: IReservedTweets;

    constructor($scope: IEditScope, $location: ng.ILocationService, reservedTweets: IReservedTweets) {
        super($scope, $location);
        this.reservedTweets = reservedTweets;
        this.$scope.tweet = <Tweet>{ TextToTweet: '' };
        this.watchCharCount();
    }

    public ok() {
        this.reservedTweets.save(this.$scope.tweet, () => this.goBack());
    }
}

app.controller('editorHomeController', EditorHomeController)
    .controller('editorAddNewController', EditorAddNewController)
    .controller('editorEditController', EditorEditController);
