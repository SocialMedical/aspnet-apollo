function uploadNewPatientFile() {
    $("#new-patientfile-description").html("Cargando " + $("#patientfile-fileinput").val() + "...");
    $("#dialog-form-patient").dialog("open");

    $("#new-patientfile-form").submit();
}

function deletePatientFile(patientfileId) {
    openDialog("¿Seguro de eliminar el registro seleccionado?", "confirmation", "deletePatientFileConfirmation(" + patientfileId + ")");
}
function deletePatientFileConfirmation(patientfileId) {
    closeDialog();
    sicPost("/Patient/DeletePatientFile", { patientFileId: patientfileId }, function (data) {
        $("#contentPanelMessage").appendTo("#patientfile-message");
        if (data.IsValid) {
            setMessage(data.Message, "success");
            $("#patientfile-item" + patientfileId).remove();
        }
        else {
            setMessage(data.Message, "error");
        }
    });
}

function updateEditPatientFile(patientfileId) {
    var name = $("[edit-content-patientfile=" + patientfileId + "] [name='patientfile_name']").val();
    var comment = $("[edit-content-patientfile=" + patientfileId + "] [name='patientfile_comment']").val();

    sicPost("/Patient/EditPatientFile", { patientFileId: patientfileId, name: name, comment: comment }, function (data) {
        $("#contentPanelMessage").appendTo("#patientfile-message");
        if (data.IsValid) {
            setMessage(data.Message, "success");
            $("[readonly-content-patientfile=" + patientfileId + "] [name='patientfile_name']").text(name.trim());
            $("[readonly-content-patientfile=" + patientfileId + "] [name='patientfile_comment']").text(comment.trim());
            cancelUpdatePatientFile(patientfileId);
        }
        else {
            setMessage(data.Message, "error");
        }
    });
}

function editPatientFile(patientfileId) {
    var name = $("[readonly-content-patientfile=" + patientfileId + "] [name='patientfile_name']").text();
    var comment = $("[readonly-content-patientfile=" + patientfileId + "] [name='patientfile_comment']").text();

    $("[edit-content-patientfile=" + patientfileId + "] [name='patientfile_name']").val(name);
    $("[edit-content-patientfile=" + patientfileId + "] [name='patientfile_comment']").val(comment.trim());

    $("[readonly-content-patientfile=" + patientfileId + "]").hide();
    $("[edit-content-patientfile=" + patientfileId + "]").show();
}

function cancelUpdatePatientFile(patientfileId) {
    $("[edit-content-patientfile=" + patientfileId + "]").hide();
    $("[readonly-content-patientfile=" + patientfileId + "]").show();
}

function uploadPatientFileFinish() {
    $("#patientfile-fileinput").val('');

    var newfile = $.parseJSON($("#upload-patinetfile_target").contents().find("#jsonResult")[0].innerHTML);
    if (newfile.IsValid == true) {
        sicGet("/Patient/PatientFile", { patientFileId: newfile.PatientFileId }, function (data) {
            $("#new-patientfile-content").html(data);
            $("#new-patientfile-content").attr('id', 'patientfile-item' + newfile.PatientFileId);
            $("#patientfile-list").prepend('<li id="new-patientfile-content" style="padding-top:20px; clear:both"></li>');
        });
    }


    setTimeout(function () {
        $("#dialog-form-patient").dialog("close");
        $("#contentPanelMessage").appendTo("#patientfile-message");

        if (newfile.IsValid == true) {
            setMessage(newfile.Message, "success");
        }
        else {
            setMessage(newfile.Message, "error");
        }

    }, 1000);
}

$(function () {
    $("#add-patientfile").click(function () {
        $("#patientfile-fileinput").val('');
        $("#new-patientfile-form [name='patientId']").val(getCurrentPatientId());
        $("#patientfile-fileinput").click();
    });

    $(".fancybox_patientfile").fancybox();

    $("#patientfile-fileinput").change(function () {
        uploadNewPatientFile();
    });

    $("#upload-patinetfile_target").load(function () {
        uploadPatientFileFinish();
    });


    $("#dialog-form-patient").dialog({
        autoOpen: false,
        show: "fast",
        height: "auto",
        width: "auto",
        modal: true,
        title: "Cargar Archivo",
        resizable: false,
        close: function () { }
    });
});