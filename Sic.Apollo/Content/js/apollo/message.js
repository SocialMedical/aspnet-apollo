function setMessage(message, type) {
    $(".message_panel").hide();
    switch (type) {
        case "error":
            $("#errorPanelMessage").show('fast');
            $("#errorMessage").text(message);
            break;
        case "information":
            $("#informationPanelMessage").show('fast');
            $("#informationMessage").text(message);
            break;
        case "success":
            $("#successPanelMessage").show('fast');
            $("#successMessage").text(message);
            break;
        case "warning":
            $("#warningPanelMessage").show('fast');
            $("#warningMessage").text(message);
            break;
    }
}