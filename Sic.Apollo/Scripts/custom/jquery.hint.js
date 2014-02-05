/* This code is licensed under Creative Commons Attribution 3.0    *
* You may share and remix the script so long as you attribute the *
* original author, Andrew January.                                *
* http://creativecommons.org/licenses/by/3.0/                     */

$(document).ready(function () {
    var isPassword = function (input) {
        return $(input).attr('realType') == 'password';
    }

    var valueIsHint = function (input) {
        return input.value == $(input).attr('hint');
    }

    var showHint = function (input, loading) {
        // FF and IE save values when you refresh the page. If the user refreshes the page
        // with the hints showing they will be the default values and the input fields won't
        // be empty. Using loading && valueIsHint is a hack to get around this and highlight
        // the hints properly on refresh.
        if (input.value == '' || (loading && valueIsHint(input))) {
            if (isPassword(input)) {
                // Must use setAttribute rather than jQuery as jQuery throws an exception
                // when changing type to maintain compatability with IE.
                // We use our own "compatability" method by simply swallowing the error.
                try {
                    input.setAttribute('type', 'input');
                } catch (e) { }
            }
            input.value = $(input).attr('hint');
            $(input).addClass('hint');
        }
    }

    var hideHint = function (input) {
        if (valueIsHint(input) && $(input).hasClass('hint')) {
            if (isPassword(input)) {
                try {
                    input.setAttribute('type', 'password');
                    // Opera loses focus when you change the type, so we have to refocus it.
                    input.focus();
                } catch (e) { }
            }

            input.value = '';
            $(input).removeClass('hint');
        }
    }

    $(':text[hint],:password[hint]').each(function (index) {
        // We change the type of password fields to text so their hint shows.
        // We need to store somewhere that they are actually password fields so we can convert
        // back when the users types something in.
        if ($(this).attr('type') == 'password') {
            $(this).attr('realType', 'password');
        }

        showHint(this, true);

        $(this).focus(function () { hideHint(this) });
        $(this).blur(function () { showHint(this, false) });
    });
});