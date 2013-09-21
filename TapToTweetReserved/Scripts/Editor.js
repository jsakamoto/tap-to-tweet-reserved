$(function () {

    // Toggle select/ unselect tweet.
    $('.reserved-tweets a').live('click', function (e) {
        e.preventDefault();
        $this = $(this);
        var isSelected = $this.hasClass('selected');
        $('.reserved-tweets a').removeClass('selected');
        $(this).toggleClass('selected', !isSelected);

        var selectedTweets = $('.reserved-tweets a.selected');
        $('footer').toggleClass({
            'no-selected-any': selectedTweets.length == 0,
            'no-selected-tweeted': selectedTweets.filter('.tweeted-is-true').length == 0,
        });
    });

    // Define generic command handler generator.
    var createHandler = function (context) {

        context = $.extend({
            beforePost: function () { return true; },
            afterPost: function () { }
        }, context || {});;

        var handler = function (e) {
            e.preventDefault();
            context.selectedTweet = $('.reserved-tweets a.selected');
            if (context.selectedTweet.length == 0) return;
            context.container = context.selectedTweet.closest('li');
            context.id = context.selectedTweet.data('id');
            context.url = $(this).attr('href') + "/" + context.id;

            if (context.beforePost() == false) return;

            $.post(context.url)
            .fail(function () { alert('Oops... something wrong.'); location.reload(); });

            context.afterPost();
        }
        return handler;
    };


    $('#cmd-edit').click(createHandler({
        beforePost: function () { location.href = this.url; return false; }
    }));

    $('#cmd-delete').click(createHandler({
        beforePost: function () { return confirm('Delete reserved tweet.\nSure?'); },
        afterPost: function () { this.container.slideUp(function () { $(this).remove(); }); }
    }));

    $('#cmd-reload').click(createHandler({
        beforePost: function () {
            return this.selectedTweet.hasClass('tweeted-is-true') && confirm('Reload the tweeted tweet.\nSure?');
        },
        afterPost: function () { this.selectedTweet.removeClass('tweeted-is-true').addClass('tweeted-is-false'); }
    }));

    // Create up/down command handler.
    var createUpDownHandler = function (method) {
        return createHandler({
            beforePost: function () {
                return (this.containerTo = method.apply(this.container)).length > 0;
            },
            afterPost: function () {
                this.container.append($('a', this.containerTo).detach());
                this.containerTo.append(this.selectedTweet.detach());
            }
        });
    };

    $('#cmd-up').click(createUpDownHandler($.fn.prev));
    $('#cmd-down').click(createUpDownHandler($.fn.next));
});