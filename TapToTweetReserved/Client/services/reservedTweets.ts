app.service('reservedTweets', ['$resource', ($resource: ngres.IResourceService) => {
    var resclass = $resource<Tweet>('/api/ReservedTweets/:id', { id: '@Id' });
    var resarray: any = resclass.query();
    resarray.createNew = data => new resclass(data);
    return resarray;
}]);
