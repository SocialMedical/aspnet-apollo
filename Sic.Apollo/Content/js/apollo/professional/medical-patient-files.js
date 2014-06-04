var LABEL_DELETE_PATIENT_FILE_MESSAGE = '¿Seguro de eliminar el registro seleccionado?';

var parent_epicrisis_patient_files = "#epicrisis_patient_files";

function uploadNewPatientFile() {
    $("#new-patientfile-description").html("Cargando " + $("#patientfile-fileinput").val() + "...");
    $("#dialog-form-patient").dialog("open");

    $("#new-patientfile-form").submit();
}

function deletePatientFile(patientfileId) {
    dialogConfirmation(LABEL_DELETE_PATIENT_FILE_MESSAGE, function () {
        deletePatientFileConfirmation(patientfileId);
    });       
}
function deletePatientFileConfirmation(patientfileId) {   
    sicPost("/Professional/PatientFiles/Delete", { patientFileId: patientfileId }, function (data) {
        sicNotifyAsPopup(data.Messages);
        if (!data.HasError) {            
            $("#patientfile-item" + patientfileId).remove();
        }        
    });
}

function updateEditPatientFile(patientfileId) {
    var name = $("[edit-content-patientfile=" + patientfileId + "] [name='patientfile_name']").val();
    var comment = $("[edit-content-patientfile=" + patientfileId + "] [name='patientfile_comment']").val();

    sicPost("/Professional/PatientFiles/Edit", { patientFileId: patientfileId, name: name, comment: comment }, function (data) {
        sicNotifyAsPopup(data.Messages);
        if (!data.HasError) {            
            $("[readonly-content-patientfile=" + patientfileId + "] [name='patientfile_name']").text(name.trim());
            $("[readonly-content-patientfile=" + patientfileId + "] [name='patientfile_comment']").text(comment.trim());
            cancelUpdatePatientFile(patientfileId);
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
    if (!newfile.HasError) {
        sicGet("/Professional/PatientFiles/Detail", { patientFileId: newfile.Content.PatientFileId }, function (data) {
            $("#new-patientfile-content").html(data);
            $("#new-patientfile-content").attr('id', 'patientfile-item' + newfile.Content.PatientFileId);
            $("#patientfile-list").prepend('<div id="new-patientfile-content"></div>');
            initializePatientFile(newfile.Content.PatientFileId);
        });
    }


    setTimeout(function () {
        $("#dialog-form-patient").dialog("close");        
        sicNotifyAsPopup(newfile.Messages);
    }, 1000);
}

function initializePatientFile(item) {
    var itemSelector = '';
    if (item != 0)
        itemSelector = ' #patientfile-item' + item;

    $(parent_epicrisis_patient_files + itemSelector + " .fancybox_patientfile").fancybox();

    $(parent_epicrisis_patient_files + itemSelector + " [edit-patient-file]").click(function () {
        var patientfileId = $(this).attr('edit-patient-file');
        $(parent_epicrisis_patient_files + " [readonly-content-patientfile='" + patientfileId + "']").hide();
        $(parent_epicrisis_patient_files + " [edit-content-patientfile='" + patientfileId + "']").show();
    });

    $(parent_epicrisis_patient_files + itemSelector + " [delete-patient-file]").click(function () {
        var patientfileId = $(this).attr('delete-patient-file');
        deletePatientFile(patientfileId);
    });

    $(parent_epicrisis_patient_files + itemSelector + " [edit-content-patientfile] button[save]").click(function () {
        var patientfileId = $(this).parents('[edit-content-patientfile]').attr('edit-content-patientfile');
        updateEditPatientFile(patientfileId);
    });

    $(parent_epicrisis_patient_files + itemSelector + " [edit-content-patientfile] button[cancel]").click(function () {
        var parentcontent = $(this).parents("[edit-content-patientfile]");
        var id = parentcontent.attr('edit-content-patientfile');
        parentcontent.hide();
        $(parent_epicrisis_patient_files + " [readonly-content-patientfile='" + id + "']").show();
    });
}

$(function () {
    $("#add-patientfile").click(function () {
        $("#patientfile-fileinput").val('');
        $("#new-patientfile-form [name='patientId']").val(getCurrentPatientId());
        $("#patientfile-fileinput").click();
    });

    initializePatientFile(0);
    
    $("#patientfile-fileinput").change(function () {
        uploadNewPatientFile();
    });

    $("#upload-patientfile_target").load(function () {
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
    $("#dialog-form-patient").parents(".ui-dialog.ui-widget").find(".ui-dialog-titlebar").hide();
});