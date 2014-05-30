var medicalcare_parent_selector = "#epicrisis_medicalcares";
var LABEL_DELETE_MEDICALCARE_MESSAGE = '¿Esta seguro de eliminar el registro del paciente?';
var LABEL_DELETE_MEDICALCARE_MESSAGE_TITLE = 'Eliminar Registro';
var LABEL_PRINT = "Imprimir";

var manyOffices = false;

function setManyOffices(value) {
    manyOffices = value;
}

function medicalCareEditOpenDialog(id) {

    sicGet("/Professional/MedicalCare/Edit", { medicalCareId: id, patientId: getCurrentPatientId() }, function (data) {
        if (data.Messages) {
            sicNotifyAsPopup(data.Messages);
        } else {
            $("#content_edit_medical_care").html(data);

            $("#dialog_edit_medical_care").dialog({
                autoOpen: false,
                show: {
                    effect: "fade",
                    duration: 400,
                    direction: "up"
                },
                hide: {
                    effect: "fade",
                    duration: 400,
                    direction: "down"
                },
                height: "auto",
                width: "auto",
                modal: true,
                dialogClass: 'noTitleStuff',
                resizable: false,
                close: function () { }
            });
            $("#content_edit_medical_care").parents(".ui-dialog.ui-widget").find(".ui-dialog-titlebar").hide();
            $("#dialog_edit_medical_care").dialog("open");

            var day = $("#content_edit_medical_care").parents("[mceditcontent]").attr('day');
            var month = $("#content_edit_medical_care").parents("[mceditcontent]").attr('month');
            var year = $("#content_edit_medical_care").parents("[mceditcontent]").attr('year');

            var evolutionDate = new Date(year, month - 1, day);
            var currentDate = Date.now();

            $("#content_edit_medical_care #medicalCareDateInput").datepicker({
                dateFormat: SIC_DATE_DISPLAY_FORMAT, maxDate: currentDate,
                defaultDate: evolutionDate
            });

            $("#content_edit_medical_care #medicalCareDateInput").datepicker("setDate", evolutionDate);


            $("#content_edit_medical_care #medicalCareDateInput").change(function () {
                $("#content_edit_medical_care #medicalCareDateLabel").text($("#content_edit_medical_care #medicalCareDateInput").val());
            });

            $("#content_edit_medical_care #medicalCareDateLabel").click(function () {
                $("#content_edit_medical_care #medicalCareDateInput").focus();
            });

            $("#content_edit_medical_care button[cancel]").click(function () {
                $("#dialog_edit_medical_care").dialog("close");
            });

            $("#content_edit_medical_care button[save]").click(function () {
                updateMedicalCare(id, false);
            });

            $("#content_edit_medical_care button[print]").click(function () {
                updateMedicalCare(id, true);
            });           

            $("#content_edit_medical_care .vademecumSearch").autocomplete({
                source: '/Professional/MedicalCare/VademecumsAutocomplete',
                minLength: 3,
                open: function () {
                    $(this).autocomplete('widget').css('z-index', 100000000);
                    return false;
                },
                change: function (event, ui) {
                    if ($(this).val() == '') {
                        $(this).parent('[medicineitem]').attr('pvid', '0');
                        $(this).parent('[medicineitem]').attr('gid', '0');
                    }
                },
                search: function (event, ui) {
                    if ($(this).val() != '') {
                        $(this).parent('[medicineitem]').attr('pvid', '0');
                        $(this).parent('[medicineitem]').attr('gid', '0');
                    }
                },
                select: function (event, ui) {
                    if (ui.item.label.substring(0, 1) == '*')
                        $(this).val(ui.item.label.substring(1));
                    else
                        $(this).val(ui.item.label);

                    var pos = $(this).parents('[medicineitem]').find("[name='posology']")
                    var quantity = $(this).parents('[medicineitem]').find("[name='quantity']")
                    $(pos).val(ui.item.pos);

                    var pvid = ui.item.id;
                    var gid = ui.item.gid;
                    if (pvid === undefined)
                        pvid = '0';
                    if (gid === undefined)
                        gid = '0';
                    $(this).parents('[medicineitem]').attr('pvid', pvid);
                    $(this).parents('[medicineitem]').attr('gid', gid);

                    $(quantity).val(1);
                    $(pos).focus();
                    $(pos).select();
                    return false;
                }
            });

        }
    });
}

function initializeMedicalCareRow(id) {
    var selector = medicalcare_parent_selector;
    if (id != 0)
        selector = medicalcare_parent_selector + " #medicalcareitemrow" + id;

    $(selector + " [mc_action_edit]").click(function () {
        var id = $(this).parents("[mcid]").attr('mcid');
        medicalCareEditOpenDialog(id);
    });

    $(selector + " [mc_action_delete]").click(function () {
        var id = $(this).parents("[mcid]").attr('mcid');
        deleteMedicalCare(id, getCurrentPatientId());
    });

    $(selector + " [mc_action_print]").click(function () {
        var id = $(this).parents("[mcid]").attr('mcid');
        printMedications(id);
    });
}

$(function () {

    initializeMedicalCareRow(0);
  
    //dialog print
    $("#officePrintSelectDialog button[print]").click(function(){
        printMedicationsConfirmation();
    });

    $("#officePrintSelectDialog button[cancel]").click(function(){
        $("#officePrintSelectDialog").dialog("close");
    });

});

function deleteMedicalCare(medicalCareId, patientId) {
    dialogConfirmation(LABEL_DELETE_MEDICALCARE_MESSAGE, function () {
        deleteMedicalCareConfirmation(medicalCareId);
    });    
}

function deleteMedicalCareConfirmation(medicalCareId) {    
    sicPost("/Professional/MedicalCare/Delete", { medicalCareId: medicalCareId, patientId: getCurrentPatientId() },
        function (data) {

            sicNotifyAsPopup(data.Messages);

            if (!data.Messages.HasError) {
                $("#medicalCareItem" + medicalCareId).empty();
                //$("#medicalcareitemrow" + medicalCareId).removeClass("medicalcare_item");
                $("#medicalcareitemrow" + medicalCareId).hide();
                refreshResumeEpicrisis(getCurrentPatientId(), 'medicalCare');
            }

        });
}

function updateMedicalCare(medicalCareId, print) {
    var evolution = $("#newEvolution").val();
    var diagnostic = $("#newDiagnostic").val();
    var treatment = $("#newTreatment").val();
    var dateTicks = sicGetTicks($("#medicalCareDateInput").datepicker("getDate"));

    var medicalCareUpdate = {
        PatientId: getCurrentPatientId(),
        MedicalCareId: medicalCareId,
        Evolution: evolution, Diagnostic: diagnostic, Treatment: treatment,
        EvolutionDateTicks: dateTicks, Medications: new Array()
    };

    $.each($("[medicineitem]"), function (i, val) {
        var medname = $.trim($(val).find("[name='medicineName']").val());
        if (medname != '') {
            var q = $.trim($(val).find("[name='quantity']").val());
            var pvid = $(val).attr("pvid");
            var gid = $(val).attr("gid");
            if (q == '') q = '1';
            if (pvid === undefined || pvid == '') pvid = '0';
            if (gid === undefined || gid == '') gid = '0';

            var posology = $(val).find("[name='posology']").val();
            var priority = $(val).attr("medicineitem");
            var mid = $(val).attr("medicineid");

            medicalCareUpdate.Medications.push({
                MedicationName: medname, Posology: posology, ProfessionalVademecumId: pvid,
                GeneralVademecumId: gid, Quantity: q, Priority: priority, MedicalCareMedicationId: mid
            });
        }
    });

    

        sicJSONDataPost("/Professional/MedicalCare/Update", { medicalCare: medicalCareUpdate }, function (data) {

            sicNotifyAsPopup(data.Messages);

            $("#dialog_edit_medical_care").dialog("close");

            if (!data.HasError) {
                sicGet("/Professional/MedicalCare/Detail", { medicalCareId: data.Content.Content.MedicalCareId, patientId: getCurrentPatientId() }, function (content) {
                    if (medicalCareId == 0) {
                        $("#newMedicalCare").html(content);
                        $("#newMedicalCare").attr('id', 'medicalcareitemrow' + data.Content.Content.MedicalCareId);
                        $('#medicalCareList').prepend('<div id="newMedicalHistory"></div>');
                    } else {
                        $("#medicalcareitemrow" + data.Content.Content.MedicalCareId).html(content);
                    }                    

                    initializeMedicalCareRow(data.Content.Content.MedicalCareId);

                    setTimeout(function () {
                        if (print)
                            printMedications(data.Content.Content.MedicalCareId);
                                                    
                    }, 200);
                   
                    refreshResumeEpicrisis(getCurrentPatientId(), 'medicalCare');
                });
            }
        });
    
}

var printMedicalCareId;
function printMedications(medicalCareId){
    printMedicalCareId = medicalCareId;
    if (manyOffices) {
        $("#officePrintSelectDialog").dialog({
            autoOpen: false,
            show: {
                effect: "drop",
                duration: 400,
                direction: "up"
            },
            hide: {
                effect: "drop",
                duration: 400,
                direction: "down"
            },
            height: "auto",
            width: "auto",
            modal: true,
            title: LABEL_PRINT,
            resizable: false
        });
        $("#officePrintSelectDialog").parents(".ui-dialog.ui-widget").find(".ui-dialog-titlebar").hide();
        $("#officePrintSelectDialog").dialog("open");
    }
    else
        printMedicationsConfirmation();
}

function printMedicationsConfirmation() {
    $("#officePrintSelectDialog").dialog("close");
    var contactLocationId = $("#officePrintSelectDialog #officePrintSelect").val();
    var url = "/Professional/MedicalCare/PrintPatientMedication?medicalCareId=" + printMedicalCareId + "&contactLocationId=" + contactLocationId;
    window.open(url);    
}