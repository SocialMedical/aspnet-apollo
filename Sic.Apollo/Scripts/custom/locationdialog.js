function setLocationDialog(contactLocationId, markerIndex, title, latitudeCenter, longitudeCenter, width, height) {
    sicGet("/Professional/SetLocation", { contactLocationId: contactLocationId, markerIndex: markerIndex }, function (data) {                
              
        $("#setLocationDialog").dialog({ autoOpen: false,
            show: "fast",
            height: height,
            width: width,
            modal: true,
            title: title,
            resizable: false,
            open: function () {
                $("#setLocationDialog").html(data);

                google.maps.event.trigger(map, 'resize');                                

//                if(latitudeCenter != -1 && longitudeCenter != -1)
//                    googleMapEdit.setCenter(new google.maps.LatLng(latitudeCenter, longitudeCenter));                
            },
            close: function () { }
        });
        $("#setLocationDialog").dialog("open");
    });                
}

    function postSetLocation(contactLocationId){
        var longitude = $("#LongitudeString").val();
        var latitude = $("#LatitudeString").val();
        sicPost("/Professional/SetLocation", { contactLocationId: contactLocationId, LongitudeString: longitude, LatitudeString: latitude }, function (data) {
            closeSetLocationDialog();
            setMessage(data.Message, data.MessageType);
            sicGet("/Professional/ProfessionalOfficeLocations", null, function (data) {
                $("#content_map").html(data);
            });
        });
    }

    function closeSetLocationDialog() {
        $("#setLocationDialog").dialog("close");
    }