var vitalsign_value_selector = "#epicrisis_vitalsign input[name=vitalsign-value]";

/*VitalSign*/

$(function () {
    var currentDate = Date.now();

    $("#vitalSignDateInput").datepicker({
        dateFormat: SIC_DATE_DISPLAY_FORMAT, maxDate: currentDate,
        defaultDate: currentDate
    });

    $("#vitalSignDateInput").datepicker("setDate", currentDate)

    $("#vitalSignDateInput").change(function () {
        $("#vitalSignDateLabel").text($("#vitalSignDateInput").val());
        clearVitalSignInputs();
    });

    $("#vitalSignDateTrigger").click(function () {
        //$("#vitalSignDateInput").show();
        $("#vitalSignDateInput").focus();

    });

    $("#btvitalsignupdate").click(function () {
        updateVitalSignValues();
    });

    $("#searchVitalSign").keyup(function () {
        var word = $(this).val().trim();
        if (word.length > 0) {
            $("#epicrisis_vitalsign [vsc]").hide();
            $.each($("#epicrisis_vitalsign [vs]"), function (i, val) {
                if ($(val).text().toLowerCase().indexOf(word.toLowerCase()) != -1) {
                    $(val).parents("[vsc]").show();
                }
            });
        }
        else {
            $("#epicrisis_vitalsign [vsc]").show();
        }
    });

});

function clearVitalSignInputs() {
    $.each($("#epicrisis_vitalsign input[name=vitalsign-value]"), function (i, val) {
        if ($(val).attr("patientVitalSignId") != 0) {
            $(val).val('');
            $(val).attr('patientVitalSignId', '0');
        }
    });
}

function updateVitalSignValues() {
    var dateTicks = sicGetTicks($("#vitalSignDateInput").datepicker("getDate"));

    var values = new Array();

    $.each($("input[name=vitalsign-value]"), function (i, val) {
        var patientVitalSignId = $(val).attr("patientVitalSignId");
        var vitalSignId = $(val).attr("vitalSignId");
        var unit = $(val).attr("unit");
        var value = $(val).val();

        values.push({ VitalSignId : vitalSignId, MeasuringUnit : unit, PatientVitalSignId: patientVitalSignId, Value : value });      
    });

    sicJSONDataPost("/Professional/VitalSign/UpdateVitalSigns", { patientId: getCurrentPatientId(), vitalSigns: values, dateTicks: dateTicks },

		function (data) {		    
		    sicNotifyAsPopup(data.Messages);
		    refreshVitalSignHistory(getCurrentPatientId());
		    refreshResumeEpicrisis(getCurrentPatientId(), 'vitalSign');
		});
}

function refreshVitalSignHistory(patientId) {
    sicGet("/Professional/VitalSign/VitalSignsHistory", { patientId: patientId }, function (data) {
        $("#vitalSignHistory").html(data);
    });
}

/*Formule VitalSign*/

$(function () {

    $(vitalsign_value_selector).change(function () {

        /*IMC*/
        if ($(this).attr('code') == 'HEIGHT' || $(this).attr('code') == 'WEIGHT') {

            var height = sicConvertToFloat($(vitalsign_value_selector + "[code='HEIGHT']").val());
            var weight = sicConvertToFloat($(vitalsign_value_selector + "[code='WEIGHT']").val());

            var r_imc = sicMathRound(weight / ((height / 100.00) * (height / 100.00)), 2);

            $(vitalsign_value_selector + "[code='IMC']").val(r_imc);
        }
    });
});