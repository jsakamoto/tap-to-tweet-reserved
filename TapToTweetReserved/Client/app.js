var app = angular.module('app', ['ngResource', 'ngRoute']);
app.config(['$httpProvider', '$routeProvider', function ($httpProvider, $routeProvider) {
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
    }]);
app.run(['$rootScope', function ($rootScope) {
        $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
            $rootScope.title = current.$$route.title;
        });
    }]);
//# sourceMappingURL=app.js.map