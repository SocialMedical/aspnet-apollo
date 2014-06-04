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

    $("#login_box_LogonName").keyup(function (event) {
        if (event.keyCode == 13) {
            $("#login_box_Password").focus();
        }
    });
    $("#login_box_Password").keyup(function (event) {
        if (event.keyCode == 13) {
            $("#loginsubmit").click();
        }
    });

    $("#loginsubmit").click(
        function () {
            sicPost("/Account/LoginVerify",
                {
                    logonName: $('#login_box_LogonName').val(),
                    password: $('#login_box_Password').val(),
                    loginFor: $('#login_box_For').val(),
                },
                function (data, status, xhr) {
                    if (data.Content.Content.Success) {
                        if (data.Content.Content.Location != '') {
                            location.href = data.Content.Content.Location;
                        }
                        else {
                            $('#login_box').hide();
                            sicGet("/Account/LoginButton", null, function (data) {
                                $('#LoginSection').html(data);
                            });
                            sicGet("/Account/Menu", null, function (data) {
                                $('#MenuSection').html(data);
                            });
                        }
                    } else {
                        $('#loginMessage').text(data.Message.Text);
                    }
                });
        }
    );
});