var medicalHistory_parent_selector = "#epicrisis_medical_history";

$(function () {

   

    $("#searchMedicalHistory").keyup(function () {
        filterMedicalHistory();
    });

    $(medicalHistory_parent_selector + " [sectionbutton] button").click(function () {
        var type = $(this).attr("medicalproblemtype");
        updateMedicalHistories(type);
    });


    $(medicalHistory_parent_selector + " textarea[name=medicalproblem]").keypress(function () {
        $(this).parents("[medicalproblemcontent]").find("[mpa]").show();
    });

    $(medicalHistory_parent_selector + " [medicalproblemcontent] button[save]").click(function () {
        var obj = $(this).parents("[medicalproblemcontent]").find("textarea[name=medicalproblem]");
        var medicalProblemId = obj.attr("medicalproblemid");
        var description = obj.val();

        updateMedicalHistory(medicalProblemId, description);
        $(this).parents("[medicalproblemcontent]").find("[mpa]").hide();
    });

    $(medicalHistory_parent_selector + " [medicalproblemcontent] button[cancel]").click(function () {
        $(this).parents("[medicalproblemcontent]").find("[mpa]").hide();        
    });
});

function filterMedicalHistory() {
    var word = $("#searchMedicalHistory").val().trim();
    if (word.length > 0) {
        $(medicalHistory_parent_selector + " .accordion-body").collapse('show');
        $(medicalHistory_parent_selector + " .accordion-toggle," + medicalHistory_parent_selector + " .accordion-inner").hide();
        $(medicalHistory_parent_selector + " [medicalProblemContent], .separation_box_historic_patient, " + medicalHistory_parent_selector + " [sectionbutton]").hide();


        $.each($(medicalHistory_parent_selector + " [medicalProblemContent]"), function (i, val) {
            if ($(val).attr('medicalProblemContent').sicContainWords(word)) {
                $(val).show();
                $(val).parents(".accordion-group").find(".accordion-toggle").show();
                $(val).parents(".accordion-inner").show();
            }
        });
    }
    else {
        $(medicalHistory_parent_selector + " [medicalProblemContent], " + medicalHistory_parent_selector + " .separation_box_historic_patient, " + medicalHistory_parent_selector + " [sectionbutton]").show();
        $(medicalHistory_parent_selector + " .accordion-toggle, " + medicalHistory_parent_selector + " .accordion-inner").show();
        $(medicalHistory_parent_selector + " .accordion-body").collapse('hide');
    }
}

function updateMedicalHistory(medicalProblemId, description) {
    sicPost("/Professional/MedicalHistory/UpdateMedicalProblem", { patientId: getCurrentPatientId(), medicalProblemId: medicalProblemId, description: description },
    function (data) {
        sicNotifyAsPopup(data.Messages);        
        refreshResumeEpicrisis(getCurrentPatientId(), 'history');
    });
}

function updateMedicalHistories(type) {
    var values = new Array();

    $.each($(medicalHistory_parent_selector + " [name=medicalproblem][medicalproblemtype=" + type + "]"), function (i, val) {

        var medicalHistoryId = $(val).attr("medicalhistoryid");
        var medicalProblemId = $(val).attr("medicalproblemid");
        var description = $(val).val();
        
        values.push({ MedicalHistoryId: medicalHistoryId, MedicalProblemId: medicalProblemId, Description: description });

    });    

    sicJSONDataPost("/Professional/MedicalHistory/UpdateMedicalProblems", { patientId: getCurrentPatientId(), medicalProblemType : type, medicalProblems: values },
    function (data) { 
        sicNotifyAsPopup(data.Messages);
        refreshResumeEpicrisis(getCurrentPatientId(), 'history');
    });
}