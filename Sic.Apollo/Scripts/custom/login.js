var mouse_is_inside = false;

$(document).ready(function () {
    $("#loginButton").click(function () {
        var loginBox = $("#content_login_box");
        if (loginBox.is(":visible"))
            loginBox.fadeOut("fast");
        else {
            loginBox.fadeIn("fast");             
            $("#login_box_LogonName").focus();
            $("#loginMessage").text("");
        }
        return false;
    });

    $("#content_login_box").hover(function () {
        mouse_is_inside = true;
    }, function () {
        mouse_is_inside = false;
    });

    $("body").click(function () {
        if (!mouse_is_inside) $("#content_login_box").fadeOut("fast");
    });
});