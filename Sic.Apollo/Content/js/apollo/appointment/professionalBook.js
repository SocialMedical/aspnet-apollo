var currentContactLocationId = 0; 
var currentStartDate; 
var currentDuration;
var currentStartTime;
var currentEndTime;
var currentAppointmentTransactionId = 0; 
    
$(function() { 
    $( "#datepicker" ).datepicker({ onSelect: function(dateText) {         
        var currentSelectDate = $( "#datepicker" ).datepicker("getDate"); 
        var ticks = getTicks(currentSelectDate); GoDate(ticks); 
    }});
          
    $("#newAppointmentReference").autocomplete({
        source: '/Patient/PatientsAutocomplete',
        minLength: 1,
        search: function(event, ui) {
            changeAppointmentReference(false);
        },
        change: function( event, ui ) {
            changeAppointmentReference(true);        
        },
        select: function (event, ui) {
            $("#newAppointmentPatientId").val(ui.item.id);    
            $("#saveAsNewPatient").hide();
        }
    }); 
            
                      
}); 

function setConfigurationTip(appointmentTransactionId, side) {
    if (side == 1) {
        $(".triggertooltip" + appointmentTransactionId).tooltip({ events: { def: "click,dblclick", tooltip: "mouseenter" }, relative: true, offset: [-80, 8], position: 'bottom right', tip: '#tooltip' + appointmentTransactionId, onBeforeShow: function () { currentAppTip(appointmentTransactionId) } });
    }
    else {
        $(".triggertooltip" + appointmentTransactionId).tooltip({ events: { def: "click,dblclick", tooltip: "mouseenter" }, relative: true, offset: [-80, -8], position: 'bottom left', tip: '#tooltip' + appointmentTransactionId, onBeforeShow: function () { currentAppTip(appointmentTransactionId) } });
    }
    if ($('#tooltipOverFlow' + appointmentTransactionId).length != 0) {
        $(".childTriggertooltip" + appointmentTransactionId).tooltip({ events: { def: "click,dblclick", tooltip: "mouseenter" }, relative: true, offset: [5, 0], position: 'bottom center', tip: '#tooltipOverFlow' + appointmentTransactionId, onBeforeShow: function () { } });
    }
}

function changeAppointmentReference(onlyEmpty){
    if((onlyEmpty == true && $("#newAppointmentReference")=='') || (onlyEmpty == false &&$("#newAppointmentReference")!=''))
    {
        $("#newAppointmentPatientId").val('');            
        $("#saveAsNewPatient").show();  
    }
}
function openNewAppointmentDialog(starDate,endDate,datetime,side, over){
       
    if($("#appointmentForNew"+ starDate).attr('isEmpty')=='true' || over)
    {        
        currentAppTip(0);
        $("#newAppointmentStartDate").val(starDate); 
        $("#newAppointmentEndDate").val(endDate);
        $("#newAppointmentSide").val(side); 
        $("#newAppointmentDateTime").text(datetime);
        
        $("#newAppointmentReferenceSaveAsNewPatient").attr('checked','checked');

        $("#newAppointmentReference").val(''); 
        $("#newAppointmentNotes").val('');
        $("#newAppointmentPatientId").val(0);

        $("#newAppointmentDialog").dialog({ autoOpen: false, height: "auto", width: "auto", modal: true, title: "Crear Nueva Cita", resizable: false, close: function() { }});
        $( "#newAppointmentDialog" ).dialog( "open" ); 
    }       
} 
       
function postCreateNewApp() { 
    if($("#newAppointmentForm").valid()==true)
    {        
        var startDateTime = $("#newAppointmentStartDate").val(); 
        var endDateTime = $("#newAppointmentEndDate").val();
        var reference = $("#newAppointmentReference").val(); 
        var officeId = $("#newAppointmentOfficeSelect :selected").val();
        var side = $("#newAppointmentSide").val(); 
        var notes = $("#newAppointmentNotes").val();
        var patientId= $("#newAppointmentPatientId").val();
        var saveAsNewPatient = $("#newAppointmentReferenceSaveAsNewPatient").is(':checked');
        closeNewAppointmentDialog();
        sicPost("/Appointment/Create", 
    { 
        contactLocationId: officeId, startDate: startDateTime, endDate: endDateTime, customerNameReference: reference, patientId : patientId, saveAsNewPatient: saveAsNewPatient,notes:notes }, 
    function (data) {             
        if(data.Success) 
        { 
            setMessage(data.Message,"success");                 
            createAppointment(data.AppointmentTransactionId,startDateTime,side);
            $("#patientCount").text(data.PatientCount);
            $("#appointmentPendingCount").text(data.AppointmentPendingCount);
        } 
        else
        {
            setMessage(data.Message,"error"); 
        }
    });
    } 
}

function postDeleteAppointment(appointmentTransactionId) {
    sicPost("/Appointment/Delete", { appointmentTransactionId: appointmentTransactionId } ,function(data)
    { 
        if(data.Success) 
        {            
            setMessage(data.Message, "success");
            refreshCalendar();
            //$("#appointmentTransactionContent" + appointmentTransactionId).empty();
            $("#appointmentPendingCount").text(data.AppointmentPendingCount);
        } 
        else
        {
            setMessage(data.Message,"error"); 
        }
    });
}

function closeNewAppointmentDialog(){ 
    $( "#newAppointmentDialog").dialog( "close" ); } 
        
function loadCalendar(contactLocationId, startDate, duration,startTime, endTime) 
{ 
    currentContactLocationId = contactLocationId; 
    currentStartDate= startDate; 
    currentDuration = duration; 
    currentStartTime = startTime; 
    currentEndTime= endTime; 
    refreshCalendar(); 
} 
    
function GoDate(startDate){ 
    currentStartDate = startDate;
    refreshCalendar(); 
} 
    
function refreshOffice(contactLocationId){ 
    currentContactLocationId = contactLocationId; 
    $(".office_active").attr("class","office_inactive"); 
    $("#officeButton"+ contactLocationId).attr("class","office_active"); 
    refreshCalendar(); 
}        
    
function currentAppTip(appointmentTransactionId){         
    if(currentAppointmentTransactionId!=0) 
    {
        closeTip(currentAppointmentTransactionId);        
    } 
    currentAppointmentTransactionId = appointmentTransactionId;                     
} 

function closeTip(appointmentTransactionId){         
    if($(".triggertooltip"+ appointmentTransactionId))
    {
        var api = $(".triggertooltip"+ appointmentTransactionId).data("tooltip");        
        api.hide();

        if($("#tooltipOverFlow"+ appointmentTransactionId).length>0)
        {
            var apiOverFlow = $(".childTriggertooltip"+ appointmentTransactionId).data("tooltip");        
            apiOverFlow.hide();
        }
    }
} 

function refreshCalendar(){ 
    currentAppointmentTransactionId = 0;
    sicGet("/Appointment/ProfessionalBookDetails",{contactLocationId:currentContactLocationId, 
    startDate:currentStartDate,duration:currentDuration,startTime:currentStartTime,endTime:currentEndTime},
function(data){
    $('#divcalendar').html(data);
}
            );            
} 
    
function postConfirmAppToAttend(appointmentTransactionId,side,patientId,showPatient) 
{ 
    waitAppointment(appointmentTransactionId);
    sicPost("/Appointment/ConfirmToAttention", { appointmentTransactionId: appointmentTransactionId } , 
function (data) 
{                
    if(data.Success)
    {   
        loadAppointment(appointmentTransactionId,side);             
        setMessage(data.Message,"success"); 
        $("#appointmentPendingCount").text(data.AppointmentPendingCount);                
        if(showPatient)
            showPatient(patientId);
    }
    else
    {
        setMessage(data.Message,"error"); 
    }                                
}
            ); 
} 
    
function postCancelAppToAttend(appointmentTransactionId, side) { 
    waitAppointment(appointmentTransactionId);
    var notes = $("#transactionNotes" + appointmentTransactionId).val(); 
    sicPost("/Appointment/CancelToAttention",{ appointmentTransactionId : appointmentTransactionId, notes: notes } , 
function(data) { 
    if(data.Success)
    {                
        loadAppointment(appointmentTransactionId,side);
        setMessage(data.Message,"success"); 
        $("#appointmentPendingCount").text(data.AppointmentPendingCount);                    
    }
    else
    {
        setMessage(data.Message,"error"); 
    }            
}); 
} 
    
function postConfirmAttended(appointmentTransactionId,side,patientId,showPatient) { 
    waitAppointment(appointmentTransactionId);
    sicPost("/Appointment/ConfirmAttended", { appointmentTransactionId: appointmentTransactionId } , 
function (data) { 
    if(data.Success)
    {                
        loadAppointment(appointmentTransactionId,side);
        setMessage(data.Message,"success");
        $("#appointmentPendingCount").text(data.AppointmentPendingCount);
        if(showPatient)
            viewPatient(patientId);
    }
    else
    {
        setMessage(data.Message,"error"); 
    }            
}); 
} 
        
function postConfirmUnAttended(appointmentTransactionId,side) { 
    waitAppointment(appointmentTransactionId);
    var notes = $("#transactionNotes" + appointmentTransactionId).val(); 
    sicPost("/Appointment/ConfirmUnAttended",{ appointmentTransactionId : appointmentTransactionId, notes: notes } , 
function(data) { 
    if(data.Success)
    {                
        loadAppointment(appointmentTransactionId,side);
        setMessage(data.Message,"success");                     
        $("#appointmentPendingCount").text(data.AppointmentPendingCount);
    }
    else
    {
        setMessage(data.Message,"error"); 
    }            
}); 
} 
    
function showAditionalArea(appointmentTransactionId){ 
    $("#primaryArea" + appointmentTransactionId).hide();
    $("#aditionalArea" + appointmentTransactionId).show(); 
} 

function showAditionalAreaDelete(appointmentTransactionId) {
    $("#primaryArea" + appointmentTransactionId).hide();
    $("#aditionalAreaDelete" + appointmentTransactionId).show();
}


function hideAditionalArea(appointmentTransactionId){
    $("#primaryArea" + appointmentTransactionId).show(); 
    $("#aditionalArea" + appointmentTransactionId).hide();
}

function hideAditionalAreaDelete(appointmentTransactionId) {
    $("#primaryArea" + appointmentTransactionId).show();
    $("#aditionalAreaDelete" + appointmentTransactionId).hide();
}
    
function createAppointment(appointmentTransactionId, startDateTime, side){            
    if($("#appointmentForNew"+ startDateTime).is('[isEmpty]'))
    {
        $("#appointmentForNew"+ startDateTime).attr('isEmpty','false');
        $("#appointmentForNew"+ startDateTime).attr('class','hours');
    }

    sicGet("/Appointment/AppointmentBookItem",{appointmentTransactionId:appointmentTransactionId,
    startDate:currentStartDate,isForNew:true},
function(data){
    $("#appointmentForNew"+ startDateTime).html(data);        
    setConfigurationTip(appointmentTransactionId,side);
    $("#appointmentForNew"+ startDateTime).attr('id','');
});        
} 
    
function loadAppointment(appointmentTransactionId,side)
{ 
    sicGet("/Appointment/AppointmentBookItem", {appointmentTransactionId:appointmentTransactionId,startDate:currentStartDate },
function(data){
    $("#appointmentInfo" + appointmentTransactionId).html(data);    
    setConfigurationTip(appointmentTransactionId,side);
});                 
} 

function waitAppointment(appointmentTransactionId){ 
    $("#tooltip" + appointmentTransactionId + ", #tooltipOverFlow" + appointmentTransactionId).hide();
    $("#tooltip" + appointmentTransactionId).remove(); 
    $("#icon" + appointmentTransactionId).hide();
    $("#loader" + appointmentTransactionId).show();    
}                  