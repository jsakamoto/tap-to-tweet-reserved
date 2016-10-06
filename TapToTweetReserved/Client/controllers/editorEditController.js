var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
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
}(EditorEditControllerBase));
app.controller('editorEditController', ['$scope', '$location', 'reservedTweets', '$routeParams', EditorEditController]);
//# sourceMappingURL=editorEditController.js.map