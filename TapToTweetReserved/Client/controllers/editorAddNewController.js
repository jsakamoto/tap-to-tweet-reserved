var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
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
}(EditorEditControllerBase));
app.controller('editorAddNewController', ['$scope', '$location', 'reservedTweets', EditorAddNewController]);
//# sourceMappingURL=editorAddNewController.js.map