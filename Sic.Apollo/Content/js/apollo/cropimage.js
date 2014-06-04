var cropImageApi = null;
var cropFileName = null;
function cropImage(file, width, height, imgWidth ,imgHeight, title) {
    $("#cropImageDialog").dialog({ autoOpen: false,
        show: "fast",
        height: imgHeight + 130,
        width: imgWidth + 50,
        modal: true,
        title: title,
        resizable: false,
        open: function () {
            var img = document.getElementById("cropImage");
            cropFileName = file;
            if (cropImageApi == null) {
                img.src = file;
                var jcrop_api = $('#cropImage').Jcrop({
                    allowResize: false,
                    allowSelect: false,
                    boxWidth: imgWidth,
                    boxHeight: imgHeight,
                    setSelect: [0, 0, width, height],
                    minSize: [width, height],
                    maxSize: [width, height],
                    onChange: showCoords,
                    onSelect: showCoords
                }, function () {
                    cropImageApi = this;
                });
            }
            else {
                cropImageApi.setImage(file);
                cropImageApi.setSelect([0, 0, width, height]);
            }
        },
        close: function () { }
    });
    $("#cropImageDialog").dialog("open");
}

function showCoords(c) {    
    $('#xcrop').val(c.x);
    $('#ycrop').val(c.y);
    //$('#wcrop').val(c.x2);
    //$('#y2').val(c.y2);
    $('#wcrop').val(c.w);
    $('#hcrop').val(c.h);
};

function saveProfilePicture() {
    try {
        closeProfilePictureDialog();
        var fileName = cropFileName;
        var x = parseInt($("#xcrop").val());
        var y = parseInt($("#ycrop").val());
        var width = parseInt($("#wcrop").val());
        var height = parseInt($("#hcrop").val());
        sicPost("/Contact/SaveContactProfilePicture", { fileName: fileName, x: x, y: y, width: width, height: height }, function (data) {
            $("#pictureProfile").show();            
            $("#pictureProfile").attr('src', data.ThumbnailImagePath);
        });
    } catch (err) {
    alert(err.message);
    }
}

function closeProfilePictureDialog() {
    $("#cropImageDialog").dialog("close");
}