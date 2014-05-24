var currentPatientId = 0;

function setCurrentPatientId(id) {
    currentPatientId = id;
}

function getCurrentPatientId() {
    return currentPatientId;
}

function refreshResumeEpicrisis(patientId, type) {
    sicGet("/Professional/Patient/EpicrisisResume", { patientId: patientId },
function (contentResume) {
    $("#epicrisis_resume").html(contentResume);
});
}

function patientFormValidate() {
    if ($("#patientProfileForm").valid() == true)
        return true;
    else
        return false;
}

function getMedicalHistoriesValues(type) {
    var values = "<medicalHistories>";
    $.each($("textarea[name=medicalproblem][medicalproblemtype=" + type + "]"), function (i, val) {
        values += '<mhe pid="' + $(val).attr("medicalhistoryid") + '"';
        values += ' mdpi="' + $(val).attr("medicalproblemid") + '"';
        values += ' pd="' + $(val).val() + '"/>';
    });
    values += "</medicalHistories>";    
}

$(function () {
    $("#button_patient_submit").click(function () {
        if (patientFormValidate()) {
            $(this).parents("form").submit();
        }
    });
});

function saveMedicalHistories() {
    sicPost("/Professional/MedicalHistory/SaveMedicalProblem", { patientId: getCurrentPatientId(), medicalProblems: values },
    function (data) {
        $("#contentPanelMessage").appendTo("#medicalHistoryContentMessage" + type);
        setMessage(data.Message, data.MessageType);
        refreshResumeEpicrisis(getCurrentPatientId(), 'history');
    });
}