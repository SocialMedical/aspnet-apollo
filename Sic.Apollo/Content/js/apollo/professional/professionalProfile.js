var emptyProfilePicture = "/Content/images/contacts/DefaultProfessional.jpg";

$(function () {
    $(".img_profile_information_border_calendar").hover(function () {
        $(".pictureProfileActions").show();
    }, function () {
        $(".pictureProfileActions").hide();
    });
});

function viewPatient(id)
{
    location.href= "/Patient/Epicrisis?patientId=" + id;
}

function selectProfileImage() {
    $("#ImgFormProfile input:file").click();
}
function refreshProfilePicture() {
    try{
        var newImg = $.parseJSON($("#UploadTargetProfile").contents().find("#jsonResult")[0].innerHTML);
        if (newImg.IsValid == true) {              
            cropImage(newImg.ImagePath,newImg.Width,newImg.Height,800,450,"Ajuste de Foto");
            //            var img = document.getElementById("pictureProfile");
            //            img.src = newImg.ImagePath;
            //            $("#pictureProfile").hide();
            //            $("#pictureProfile").fadeIn(500, null);
        }
        $("#panelLoadingImageProfile").hide();        
    }catch(err){
    }
}

function UploadProfileImage() {
    //$("#panelLoadingImageProfile").show();
    //$("#pictureProfile").hide();
    $("#ImgFormProfile").submit();
}   

function deleteProfilePictureImage(){         
    sicPost("/Contact/DeleteProfilePicture",null, function (data) {             
        var img = document.getElementById("pictureProfile");
        img.src = emptyProfilePicture;
    });
}
