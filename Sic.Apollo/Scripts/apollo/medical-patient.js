var currentPatientId = 0;

function setCurrentPatientId(id) {
    currentPatientId = id;
}

function getCurrentPatientId() {
    return currentPatientId;
}

function refreshResumeEpicrisis(patientId, type) {
    sicGet("/Patient/Resume", { patientId: patientId },
function (contentResume) {
    $("#resumeEpicrisis").html(contentResume);
});
}
