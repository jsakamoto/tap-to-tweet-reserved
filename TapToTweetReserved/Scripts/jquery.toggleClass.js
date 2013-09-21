(function ($) {
    var base = $.fn.toggleClass;
    $.fn.toggleClass = function (map) {
        map = map || {};
        $this = this;
        if (typeof (map) != "object") base.apply(this, $.makeArray(arguments));
        else {
            for (var className in map) {
                if (map.hasOwnProperty(className)) {
                    base.apply(this, [className, map[className]]);
                }
            }
        }
        return $this;
    };
})(jQuery)