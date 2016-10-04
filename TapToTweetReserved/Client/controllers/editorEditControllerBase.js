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
}());
//# sourceMappingURL=editorEditControllerBase.js.map