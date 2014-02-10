var vitalsign_value_selector = "[vitalsign-edit-content] input[name=vitalsign-value]";

/*VitalSign*/

$(function(){
	var currentDate = Date.now();
	
	$("#vitalSignDateInput").datepicker({ dateFormat: SIC_DATE_DISPLAY_FORMAT, maxDate: currentDate,
		defaultDate: currentDate});
        
	$("#vitalSignDateInput").datepicker("setDate", currentDate)

	$("#vitalSignDateInput").change(function()
	{
		$("#vitalSignDateLabel").text($("#vitalSignDateInput").val());
		clearVitalSignInputs();
	});
        
	$("#vitalSignDateLabel").click(function()
	{
		$("#vitalSignDateInput").focus();
	});

});

function clearVitalSignInputs()
{
	$.each($("input[name=vitalsign-value]"), function (i, val){
		if($(val).attr("patientVitalSignId")!=0)
		{
			$(val).val('');
			$(val).attr('patientVitalSignId', '0');                
		}
	});
}

function getvitalSignValues() {
	var values = "<vitalSign>";
	var dateTicks = sicGetTicks($("#vitalSignDateInput").datepicker("getDate"));
	$.each($("input[name=vitalsign-value]"), function (i, val) {
		values += '<pee id="' + $(val).attr("patientVitalSignId") + '"';
		values += ' vid="' + $(val).attr("vitalSignId") + '"';
		values += ' munit="' + $(val).attr("unit") + '"';
		values += ' val="' + $(val).val() + '"/>';
	});
	values += "</vitalSign>";
        
	sicPost("/Patient/SaveVitalSign", { patientId: getCurrentPatientId(), vitalSigns: values, dateTicks: dateTicks },
	
		function (data) {
		$("#contentPanelMessage").appendTo("#vitalSignContentMessage");               
		setMessage(data.Message, data.MessageType);
		refreshVitalSignHistory(getCurrentPatientId());
		refreshResumeEpicrisis(getCurrentPatientId(), 'vitalSign');
		});

}

function refreshVitalSignHistory(patientId)
{
	sicGet("/Patient/VitalSignHistory", { patientId: patientId }, function (data)
	{
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

			var r_imc = sicMathRound(weight / ((height/100.00) * (height/100.00)),2);

			$(vitalsign_value_selector + "[code='IMC']").val(r_imc);
		}
	});
});