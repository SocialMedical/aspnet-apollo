var weekDays = null;
var startTimeOfDay = null;
var endTimeOfDay = null;
var appointmenDuration = null;
var endDate = null;
var eachWeek = null;
var startConfiguration = null; 
var startDate = null;
var appSpecializationId = null;
var locationsId = null;
var appHoraryMode = 'SEARCH';

function showMoreApp(contactLocationId)
{                
    try
    {
        $(".moreapp"+contactLocationId).show("slow");
        $(".linkmoreapp"+contactLocationId).hide();        
    }
    catch(err)
    {
        alert(err);
    }
}

function weekChange(ticks) 
{                    
    $(".appointmentEntries").hide();            
    if(appHoraryMode == 'SCHEDULE')
    {
            var startDate = ticks;  
            sicGet("/Appointment/Schedule/Preview", {contactLocationId : locationsId,
            weekDays: weekDays, startTimeOfDay: startTimeOfDay, endTimeOfDay: endTimeOfDay, appointmentDuration: appointmenDuration,
            startDate: startDate, endDate: endDate, eachWeek:eachWeek, startConfiguration:startConfiguration},
        function(data){
            $('#divappointments').html(data);
            $(".appointmentEntries").fadeIn('slow');
        });
    }
    else if(appHoraryMode=='SEARCH')
    {
        sicGet("/Appointment/Booking/Search", {
            locationsId:locationsId,specializationId:appSpecializationId,startTime:ticks
        }, function(data){
            $('#divappointments').html(data);
            $(".appointmentEntries").fadeIn('slow');
        });            
    }    
} 