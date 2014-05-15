var MESSAGE_TYPE_INFORMATION = 0;
var MESSAGE_TYPE_ERROR = 1;
var MESSAGE_TYPE_WARNING = 2;
var MESSAGE_TYPE_CONFIRMATION = 3;
var MESSAGE_TYPE_SUCCESS = 4

var DIALOG_POSITIVE_LABEL_BUTTON = 'Sí';
var DIALOG_NEGATIVE_LABEL_BUTTON = 'No';
var DIALOG_OK_LABEL_BUTTON = 'Aceptar';
var DIALOG_CANCEL_LABEL_BUTTON = 'Cancelar';

var DIALOG_BUTTONS_POSITIVE_NEGATIVE_CANCEL = 'YES-NO-CANCEL';
var DIALOG_BUTTONS_POSITIVE_NEGATIVE = 'YES-NO';

function sicDialogMessage(message, title, modal, callback) {

    var buttons = [{
        id: DIALOG_OK_LABEL_BUTTON,
        text: DIALOG_OK_LABEL_BUTTON,
        click: function () {
            $(this).dialog("close");
            if (callback)
                callback();
        }
    }];

    sicDialog(message, title, modal, buttons);
}

function sicDialogYesNoConfirmation(message, title, modal, positiveCallback, negativeCallback) {

    var buttons = [{
        id: DIALOG_POSITIVE_LABEL_BUTTON,
        text: DIALOG_POSITIVE_LABEL_BUTTON,
        click: function () {
            $(this).dialog("close");
            if (positiveCallback)
                positiveCallback();
        }
    }, {
        id: DIALOG_NEGATIVE_LABEL_BUTTON,
        text: DIALOG_NEGATIVE_LABEL_BUTTON,
        click: function () {
            $(this).dialog("close");
            if (negativeCallback)
                negativeCallback();
        }

    }];

    sicDialog(message, title, modal, buttons);
}

function sicDialog(message, title, modal, buttons) {
    if ($("#sic-dialog").length == 0) {
        var html = '<div id="sic-dialog" title="' + title + '" style=\"display:none\">';
        html += message;
        html += '</div>';
        $('body').append(html);
    } else {
        $("#sic-dialog").html(message);
        $("#sic-dialog").attr('title', title);
        $("#sic-dialog").parent().find("span.ui-dialog-title").html(title);
    }

    if (buttons == null || buttons == undefined) {
        buttons = {
            DIALOG_POSITIVE_LABEL_BUTTON: function () {
                $(this).dialog("close");
                if (positiveCallback)
                    positiveCallback();
            },
            DIALOG_NEGATIVE_LABEL_BUTTON: function () {
                $(this).dialog("close");
                if (negativeCallback)
                    negativeCallback();
            }
        }
    }

    $("#sic-dialog").dialog({
        autoOpen: false,
        modal: modal,
        resizeable: false,
        buttons: buttons,
        show: {
            effect: "drop",
            duration: 400,
            direction: "up"
        },
        hide: {
            effect: "drop",
            duration: 400,
            direction: "down"
        }
    });

    $("#sic-dialog").dialog("open");
}

function sicNotify(message, type, title, popup, popupRemain, canClose) {
    if (popup) {
        sicStickyNotify(message, type, title, popupRemain);
    }
    else {
        sicNotifyBlock(message, type, title, canClose);
    }
}

function sicNotifySuccessAsBlock(message, title) {
    sicNotifyAsBlock(message, MESSAGE_TYPE_SUCCESS, title);
}

function sicNotifyInformationAsBlock(message, title) {
    sicNotifyAsBlock(message, MESSAGE_TYPE_INFORMATION, title);
}

function sicNotifyErrorAsBlock(message, title) {
    sicNotifyAsBlock(message, MESSAGE_TYPE_ERROR, title);
}

function sicNotifyWarningAsBlock(message, title) {
    sicNotifyAsBlock(message, MESSAGE_TYPE_WARNING, title);
}

function sicNotifyAsBlock(message, type, title, canClose) {

    if (message instanceof Array) {
        for (var i = 0; i < message.length; i++)
            sicNotifyAsBlock(message[i], type, title, canClose);
    } else if (message instanceof Object) {
        sicNotifyAsBlock(message.TextMessage, message.MessageType, message.Title, canClose);
    } else {

        var htmlContent = ''
        var classMessage = '';

        switch (type) {
            case MESSAGE_TYPE_SUCCESS:
                classMessage = 'alert-success';
                break;
            case MESSAGE_TYPE_ERROR:
                classMessage = 'alert-danger';
                break;
            case MESSAGE_TYPE_WARNING:
                classMessage = 'alert-warning';
                break;
            default:
                classMessage = 'alert-info';
                break;
        }

        var useBlock = false;
        if (title != undefined && title != '') {
            useBlock = true;
            classMessage += ' alert-block';
        }

        htmlContent += '<div class="alert ' + classMessage + '">';
        if (canClose) {
            if (!useBlock)
                htmlContent += '<button class="close" data-dismiss="alert">×</button>';
            else
                htmlContent += '<a class="close" data-dismiss="alert" href="#">×</a>';
        }

        if (useBlock) {
            htmlContent += '<h4 class="alert-heading">' + title + '</h4>';
        }

        htmlContent += message;
        htmlContent += '</div>';

        $("#notification-message-content").append(htmlContent);
    }
}

function sicNotifyAsPopup(message, type, title, remain, image) {
    if (message instanceof Array) {
        for (var i = 0; i < message.length; i++)
            sicNotifyAsPopup(message[i], type, title, remain, image);
    } else if (message instanceof Object) {
        sicNotifyAsPopup(message.TextMessage, message.MessageType, message.Title, remain);
    } else {
        sicStickyNotify(message, type, title, remain, image);
    }
}

function sicNotifyInformationAsPopup(message, title, remain, image) {
    sicNotifyAsPopup(message, MESSAGE_TYPE_INFORMATION, title, remain, image);
}

function sicNotifyErrorAsPopup(message, title, remain, image) {
    sicNotifyAsPopup(message, MESSAGE_TYPE_ERROR, title, remain, image);
}

function sicNotifySuccessAsPopup(message, title, remain, image) {
    sicNotifyAsPopup(message, MESSAGE_TYPE_SUCCESS, title, remain, image);
}

function sicNotifyWarningAsPopup(message, title, remain, image) {
    sicNotifyAsPopup(message, MESSAGE_TYPE_WARNING, title, remain, image);
}

function sicStickyNotify(message, type, title, remain, image) {
    if (remain == undefined) remain = false;
    if (title == undefined || title == '') {
        title = message;
        message = ' ';
    }
    var sclass = 'info';

    if(image == undefined){
        switch (type) {
            case MESSAGE_TYPE_SUCCESS:
                image = '/Content/img/style/icon_success.png';
                sclass = 'success';
                break;
            case MESSAGE_TYPE_ERROR:
                image = '/Content/img/style/icon_error.png';
                sclass = 'error';
                break;
            case MESSAGE_TYPE_WARNING:
                image = '/Content/img/style/icon_warning.png';
                sclass = 'warning';
                break;
            case MESSAGE_TYPE_INFORMATION:
                image = '/Content/img/style/icon_info.png';
                sclass = 'info';
                break;
            default:
                sclass = 'info';
                break;
        }
    }
    
    $.gritter.add({
        title: title,
        text: message,
        image: image,
        sticky: remain,
        class_name:  sclass
    });
}