$(function () {
    var updateCmdState = function () {

    };

    $('.reserved-tweets a').live('click', function (e) {
        e.preventDefault();
        $this = $(this);
        var isSelected = $this.hasClass('selected');
        $('.reserved-tweets a').removeClass('selected');
        $(this).toggleClass('selected', !isSelected);

        $('footer').toggleClass('no-selected-tweets', ($('.reserved-tweets a.selected').length == 0));
    });

    $('#cmd-delete').click(function (e) {
        e.preventDefault();
        var selectedTweet = $('.reserved-tweets a.selected');
        if (selectedTweet.length == 0) return;
        if (!confirm('Delete reserved tweet.\nSure?')) return;

        var id = selectedTweet.data('id');
        var url = $(this).attr('href') + "/" + id;
        $container = selectedTweet.closest('li');
        $.post(url, { id: id })
        .done(function () { $container.slideUp(function () { $container.remove(); }); })
        .fail(function () { alert('Oops... something wrong.'); });
    });
});