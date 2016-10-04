app.service('reservedTweets', ['$resource', function ($resource) {
        var resclass = $resource('/api/ReservedTweets/:id', { id: '@Id' });
        var resarray = resclass.query();
        resarray.createNew = function (data) { return new resclass(data); };
        return resarray;
    }]);
//# sourceMappingURL=reservedTweets.js.map