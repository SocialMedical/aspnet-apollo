$(function () {
    $("#searchMedicalHistory").keyup(function () {
        var word = $(this).val().trim();
        if (word.length > 0) {
            $("#epicrisis_medical_history .accordion-body").collapse('show');
            $("#epicrisis_medical_history .accordion-toggle, #epicrisis_medical_history .accordion-inner").hide();

            $("#epicrisis_medical_history [medicalProblemContent], #epicrisis_medical_history .separation_box_historic_patient, #epicrisis_medical_history button[sectionbutton]").hide();
            

            $.each($("#epicrisis_medical_history [medicalProblemContent]"), function (i, val) {
                if ($(val).attr('medicalProblemContent').toLowerCase().indexOf(word.toLowerCase()) != -1) {
                    $(val).show();                    
                    $(val).parents(".accordion-group").find(".accordion-toggle").show();
                    $(val).parents(".accordion-inner").show();
                }
            });            
        }
        else {
            $("#epicrisis_medical_history [medicalProblemContent], #epicrisis_medical_history .separation_box_historic_patient, #epicrisis_medical_history button[sectionbutton]").show();
            $("#epicrisis_medical_history .accordion-toggle, #epicrisis_medical_history .accordion-inner").show();
            $("#epicrisis_medical_history .accordion-body").collapse('hide');
        }
    });

    $("#epicrisis_medical_history button[sectionbutton]").click(function () {
        var type = $(this).attr("medicalproblemtype");
        updateMedicalHistories(type);
    });


    $("#epicrisis_medical_history textarea[name=medicalproblem]").keypress(function () {
        $(this).parents("[medicalproblemcontent]").find("[mpa]").show();
    });

    $("#epicrisis_medical_history [medicalproblemcontent] button[save]").click(function () {
        var obj = $(this).parents("[medicalproblemcontent]").find("textarea[name=medicalproblem]");
        var medicalProblemId = obj.attr("medicalproblemid");
        var description = obj.val();

        updateMedicalHistory(medicalProblemId, description);
        $(this).parents("[medicalproblemcontent]").find("[mpa]").hide();
    });

    $("#epicrisis_medical_history [medicalproblemcontent] button[cancel]").click(function () {
        $(this).parents("[medicalproblemcontent]").find("[mpa]").hide();        
    });
});


function updateMedicalHistory(medicalProblemId, description) {
    sicPost("/Professional/MedicalHistory/UpdateMedicalProblem", { patientId: getCurrentPatientId(), medicalProblemId: medicalProblemId, description: description },
    function (data) {
        sicNotifyAsPopup(data.Messages);        
        refreshResumeEpicrisis(getCurrentPatientId(), 'history');
    });
}

function updateMedicalHistories(type) {
    var values = new Array();

    $.each($("#epicrisis_medical_history [name=medicalproblem][medicalproblemtype=" + type + "]"), function (i, val) {

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