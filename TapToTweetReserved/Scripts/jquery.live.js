(function ($) {
	$.fn.live = $.fn.live || function (event, callback) {
		$this = this;
		$(document).on(event, $this.selector, callback);
		return $this;
	};
})(jQuery)