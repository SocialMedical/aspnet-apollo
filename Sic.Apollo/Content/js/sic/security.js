function sicSecurityLogIn(logonName, password, callback) {    
    $.post("/Security/PartialLogIn", { logonNameOrEmail: logonName, password: password },
        function(data){
            callback(data);
        });            
}

function sicSecurityPasswordChange(logonName, password, newPassword,callback) {
    $.post("/Security/PartialChangePassword", { logonNameOrEmail: logonName, password: password, newPassword: newPassword },
        function (data) {
            callback(data);
        });
}