
$(function () {
    var htmlDialog = '<div id="confirmationDialog" style="display:none">';
    htmlDialog += '<div class="dialog-content">';
    htmlDialog += '<div class="row dialog-inner">';
    htmlDialog += '<div class="col-lg-12">';
    htmlDialog += '<div class="dialog-title-circle"></div>';
    htmlDialog += '<div class="dialog-title">';
    //htmlDialog += '<div class="float dialog-content-ico">';
    //htmlDialog += '     <i class="fa fa-question"></i>';
    //htmlDialog += ' </div>';
    htmlDialog += ' <div class="float dialog-title-text">Este es un mensaje</div>';
    htmlDialog += '</div>';
    htmlDialog += '</div>';
    htmlDialog += '</div>';
    htmlDialog += '<div class="dialog-buttons">';
    htmlDialog += '<button type="button" positive class="btn btn-warning btn-medium">' + DIALOG_POSITIVE_LABEL_BUTTON + '</button>';
    htmlDialog += '<button type="button" negative class="btn btn-primary btn-medium">' + DIALOG_NEGATIVE_LABEL_BUTTON + '</button>';
    htmlDialog += '</div>';
    htmlDialog += '</div>';
    htmlDialog += '</div>';

    $("body").append(htmlDialog);
    
});

function dialogConfirmation(message, positiveCallback, negativeCallback) {

    $("#confirmationDialog .dialog-title-text").text(message);

    $("#confirmationDialog").dialog({
        autoOpen: false,
        show: {
            effect: "fade",
            duration: 400,
            direction: "up"
        },
        hide: {
            effect: "fade",
            duration: 400,
            direction: "down"
        },
        height: "auto",
        width: "auto",
        modal: true,
        dialogClass: 'noTitleStuff',
        resizable: false,
        close: function () { }
    });    

    $("#confirmationDialog").parents(".ui-dialog.ui-widget").find(".ui-dialog-titlebar").hide();
    $("#confirmationDialog").dialog("open");

    $("#confirmationDialog button[positive]").click(function () {
        $("#confirmationDialog").dialog("close");
        if (positiveCallback)
            positiveCallback();
    });

    $("#confirmationDialog button[negative]").click(function () {
        $("#confirmationDialog").dialog("close");
        if (negativeCallback)
            negativeCallback();
    });
}