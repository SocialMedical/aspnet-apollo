var physicalex_parent_selector = "#epicrisis_physical_ex";

$(function () {

    var currentDate = Date.now();

    $("#physicalExaminationDateInput").datepicker({
        dateFormat: SIC_DATE_DISPLAY_FORMAT, maxDate: currentDate,
        defaultDate: currentDate
    });

    $("#physicalExaminationDateInput").datepicker("setDate", currentDate);

    $("#physicalExaminationDateInput").change(function () {
        $("#physicalExaminationDateLabel").text($("#physicalExaminationDateInput").val());
        clearPhysicalExaminations();
    });

    $("#physicalExDateTrigger").click(function () {
        $("#physicalExaminationDateInput").focus();
    });


    $("#searchPhysicalExamination").keyup(function () {
        filterPhysicalExamination();
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

function filterPhysicalExamination() {
    var word = $("#searchPhysicalExamination").val().trim();
    if (word.length > 0) {
        $(physicalex_parent_selector + " [pec]," + physicalex_parent_selector + " [sectionbutton]").hide();

        $.each($(physicalex_parent_selector + " [pen]"), function (i, val) {
            if ($(val).text().sicContainWords(word)) {
                $(val).parents("[pec]").show();
            }
        });
    }
    else {
        $(physicalex_parent_selector + " [pec]," + physicalex_parent_selector + " [sectionbutton]").show();
    }
}

function clearPhysicalExaminations() {
    $.each($(physicalex_parent_selector + " textarea[name=physicalExaminationValue]"), function (i, val) {
        if ($(val).attr("patientPhysicalExaminationId") != 0) {
            $(val).val('');
            $(val).attr('patientPhysicalExaminationId', '');
        }
    });
}

function updatePhysicalExamination(pid, id, value) {
    var dateTicks = sicGetTicks($("#physicalExaminationDateInput").datepicker("getDate"));

    sicJSONDataPost("/Professional/PhysicalExamination/Update",{
            dateTicks: dateTicks, physicalExamination: { PatientId: getCurrentPatientId(), PatientPhysicalExaminationId: id, PhysicalExaminationId: pid, Examination: value }
    },
    function (data) {
        sicNotifyAsPopup(data.Messages);
        if (!data.HasError) {
            refreshPhysicalExaminationHistory();
            refreshResumeEpicrisis(getCurrentPatientId(), 'physicalExamination');
        }
    });
}

function refreshPhysicalExaminationHistory() {
    sicGet("/Professional/PhysicalExamination/History", { patientId: getCurrentPatientId() }, function (data) {        
        if (!data.HasError) {
            $("#physicalExaminationHistory").html(data);
            filterPhysicalExamination();
        }
        else
            sicNotifyAsPopup(data.Messages);
    });    
}

function updatePhysicalExaminations() {
    var values = Array();
    var dateTicks = sicGetTicks($("#physicalExaminationDateInput").datepicker("getDate"));

    $.each($(physicalex_parent_selector + " textarea[name=physicalExaminationValue]"), function (i, val) {
        var pid = $(val).attr("physicalExaminationId");
        var id = $(val).attr("patientPhysicalExaminationId");
        var value = $(val).val();
        values.push({PatientPhysicalExaminationId : id, PhysicalExaminationId : pid,  Examination : value});
    });
   

    sicJSONDataPost("/Professional/PhysicalExamination/UpdateAll", {
        patientId: getCurrentPatientId(),
        dateTicks: dateTicks,
        physicalExaminations: values
    },
    function (data) {
        sicNotifyAsPopup(data.Messages);

        if (!data.HasError)
            refreshResumeEpicrisis(getCurrentPatientId(), 'physicalExamination');
    });
}