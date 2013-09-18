$(function () {
    $(document).on('click', '.reserved-tweets a', function (e) {
        e.preventDefault();
        var $this = $(this);
        $this.addClass('current');
        setTimeout(function () {
            if (confirm("Tweet this stock?")) {
                $container = $this.closest('li');
                $.post("/Home/Tweet", { id: $this.data("id") })
                .done(function () { $container.slideUp(function () { $container.remove(); }); })
                .fail(function () { alert('Oops... something wrong.'); $this.removeClass('current'); });
            }
            else {
                $this.removeClass('current');
            }
        }, 0);
    });
});