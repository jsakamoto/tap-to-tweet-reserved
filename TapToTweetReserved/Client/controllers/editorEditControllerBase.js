var EditorEditControllerBase = (function () {
    function EditorEditControllerBase($scope, $location, tweet) {
        this.$scope = $scope;
        this.$location = $location;
        this.tweet = tweet;
        this.watchCharCount();
    }
    EditorEditControllerBase.prototype.watchCharCount = function () {
        var _this = this;
        this.$scope.$watch(function () { return _this.tweet.TextToTweet; }, function () {
            var MAXCHARS = 140;
            _this.charCount = charCounter(_this.tweet.TextToTweet || '', MAXCHARS);
            _this.overflow = _this.charCount <= 0;
        });
    };
    EditorEditControllerBase.prototype.goBack = function () {
        this.$location.url('/');
    };
    return EditorEditControllerBase;
}());
//# sourceMappingURL=editorEditControllerBase.js.map