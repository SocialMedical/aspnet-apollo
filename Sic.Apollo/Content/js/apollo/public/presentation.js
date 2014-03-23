$(document).ready(function() {
    $('#navigation').localScroll({duration:1000});

    $(".fancyboxpictureProfile").fancybox();
    $(".fancyboxpicture").fancybox({
        nextEffect: 'fade', // 'elastic', 'fade' or 'none'			       			       			       			       
        prevEffect: 'fade', // 'elastic', 'fade' or 'none'
        helpers : {
            thumbs : {
                width: 75,
                height: 50
            },
            title : {
                type : 'over'
            }
        }                    
    });    
});
    
function viewOfficePresentation(contactLocationId, marker, specializationId) {
    $("#presentationOffice").hide();    
    sicGet("/Public/Professional/OfficePresentation",{contactLocationId:contactLocationId,specializationId:specializationId,
            marker:marker}, function(data){
    $("#presentationOffice").html(data);
    $("#presentationOffice").slideDown('slow');
    });        
}

function mapMarkerClick(professionalId,contactLocationId,markerIndex){
    viewOfficePresentation(contactLocationId,markerIndex);        
}