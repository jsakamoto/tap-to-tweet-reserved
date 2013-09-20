$(function () {
    var MAXCHARS = 140;
    var $textarea = $('textarea');
    var $charcounter = $('#char-counter');

    var updateCharCounter = function () {
        var left = MAXCHARS - $textarea.val().length;
        $charcounter
            .text(left)
            .toggleClass('overflow', left <= 0);
    };

    $textarea.on('keydown keyup keypress change click', updateCharCounter);
    updateCharCounter();
});