var parent_epicrisis_patient_files = "#epicrisis_patient_files";

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

    var newfile = $.parseJSON($("#upload-patientfile_target").contents().find("#jsonResult")[0].innerHTML);
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

    $(parent_epicrisis_patient_files + " .fancybox_patientfile").fancybox();

    $("#patientfile-fileinput").change(function () {
        uploadNewPatientFile();
    });

    $("#upload-patinetfile_target").load(function () {
        uploadPatientFileFinish();
    });

    $(parent_epicrisis_patient_files + " [edit-patient-file]").click(function () {
        var patientfileId = $(this).attr('edit-patient-file');
        $(parent_epicrisis_patient_files + " [readonly-content-patientfile='" + patientfileId + "']").hide();
        $(parent_epicrisis_patient_files + " [edit-content-patientfile='" + patientfileId + "']").show();
    });

    $(parent_epicrisis_patient_files + " [edit-content-patientfile] button[save]").click(function () {

    });

    $(parent_epicrisis_patient_files + " [edit-content-patientfile] button[cancel]").click(function () {
        var parentcontent = $(this).parents("[edit-content-patientfile]");
        var id = parentcontent.attr('edit-content-patientfile');
        parentcontent.hide();
        $(parent_epicrisis_patient_files + " [readonly-content-patientfile='" + id + "']").show();
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
    $("#dialog-form-patient").parents(".ui-dialog.ui-widget").find(".ui-dialog-titlebar").hide();
});