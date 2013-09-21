$(function () {
    $('header .menu').click(function (e) {
        e.preventDefault();
        $('header').toggleClass('expand-menu')
    });
    $(document).click(function (e) {
        if ($(e.target).hasClass('menu')) return;
        $('header').removeClass('expand-menu')
    })

    $('#sign-out').click(function (e) {
        e.preventDefault();
        if (!confirm('Sign out: Sure?')) return;

        $.post($(this).attr('href'))
        .done(function (data) { location.href = data.url; })
        .fail(function () { alert('Oops... something wrong.'); });
    });

    $(document.body).addClass('page-loaded')
});