var physicalex_parent_selector = "#epicrisis_physical_ex";

$(function () {

    var currentDate = Date.now();

    $("#physicalExaminationDateInput").datepicker({
        dateFormat: SIC_DATE_DISPLAY_FORMAT, maxDate: currentDate,
        defaultDate: currentDate
    });

    $("#physicalExaminationDateInput").datepicker("setDate", currentDate)

    $("#physicalExaminationDateInput").change(function () {
        $("#physicalExaminationDateLabel").text($("#physicalExaminationDateInput").val());
        clearPhysicalExaminations();
    });

    $("#physicalExDateTrigger").click(function () {
        $("#physicalExaminationDateInput").focus();
    });


    $("#searchPhysicalExamination").keyup(function () {
        var word = $(this).val().trim();
        if (word.length > 0) {
            $(physicalex_parent_selector + " [pec]," + physicalex_parent_selector + " [sectionbutton]").hide();

            $.each($(physicalex_parent_selector + " [pen]"), function (i, val) {
                if ($(val).text().toLowerCase().indexOf(word.toLowerCase()) != -1) {                    
                    $(val).parents("[pec]").show();
                }
            });
        }
        else {
            $(physicalex_parent_selector + " [pec]," + physicalex_parent_selector + " [sectionbutton]").show();
        }
    });

    $(physicalex_parent_selector + " [sectionbutton] button").click(function () {       
        updatePhysicalExaminations();
    });

    $(physicalex_parent_selector + " textarea[name=physicalExaminationValue]").keypress(function () {
        $(this).parents("[pec]").find("[pea]").show();
    });

    $(physicalex_parent_selector + " [pea] button[save]").click(function () {
        var obj = $(this).parents("[pec]").find("textarea[name=physicalExaminationValue]");
        var pid = obj.attr("physicalExaminationId");
        var id = obj.attr("patientPhysicalExaminationId");
        var value = obj.val();

        updatePhysicalExamination(pid,id,value);
        $(this).parents("[pec]").find("[pea]").hide();
    });

    $(physicalex_parent_selector + " [pea] button[cancel]").click(function () {
        $(this).parents("[pec]").find("[pea]").hide();
    });
});

function clearPhysicalExaminations() {
    $.each($(physicalex_parent_selector + " textarea[name=physicalExaminationValue]"), function (i, val) {
        if ($(val).attr("patientPhysicalExaminationId") != 0) {
            $(val).val('');
            $(val).attr('patientPhysicalExaminationId', '');
        }
    });
}

function updatePhysicalExamination(pid,id,value) {
    sicJSONDataPost("/Professional/PhysicalExamination/Update",
        { dateTicks: 100,  physicalExamination: { PatientId: getCurrentPatientId(), PatientPhysicalExaminationId: id, PhysicalExaminationId: pid, Examination: value }
    },
    function (data) {
        sicNotifyAsPopup(data.Messages);
        if(!data.HasError)
            refreshResumeEpicrisis(getCurrentPatientId(), 'physicalExamination');
    });
}

function updatePhysicalExaminations() {
    var values = Array();
    $.each($(physicalex_parent_selector + " textarea[name=physicalExaminationValue]"), function (i, val) {
        var peid = $(val).attr("physicalExaminationId");
        var id = $(val).attr("patientPhysicalExaminationId");
        var value = $(val).val();
        values.push({PatientPhysicalExaminationId : id, PhysicalExaminationId : pid,  Examination : value});
    });
   

    sicJSONDataPost("/Professional/PhysicalExamination/UpdateAll", {
        patientId: getCurrentPatientId(),
        dateTicks : 100,
        physicalExaminations: values
    },
    function (data) {
        sicNotifyAsPopup(data.Messages);

        if (!data.HasError)
            refreshResumeEpicrisis(getCurrentPatientId(), 'physicalExamination');
    });
}