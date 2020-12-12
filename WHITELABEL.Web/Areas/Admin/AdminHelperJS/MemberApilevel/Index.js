function DeleteInformation(id) {
    bootbox.confirm({
        //title: "Message",
        message: "Do you want to delete information",
        buttons: {
            confirm: {
                label: 'Confirm',
                className: 'btn-success btn-sm'
            },
            cancel: {
                label: 'Cancel',
                className: 'btn-danger btn-sm'
            }
        },
        callback: function (result) {
            if (result) {
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: "/MemberAPILabel/DeleteInformation?area=Admin",
                    //"@Url.Action("DeleteInformation", "MemberAPILabel", new {area = "Admin"})",
                    data: {
                        __RequestVerificationToken: token,
                        id: id
                    },                   
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {

                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Informaiton Deleted successfully",
                                backdrop: true
                            });

                        }
                        else {
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                message: "there is some thing error",
                                backdrop: true
                            });
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log(status);
                    }
                });
            }
        }
    });
}
function MemberStatus(id, statusval) {
    var statuschk = statusval;
    var msg = "";
    if (statuschk == "True") {
        msg = "Are you sure to deactivate the user";
    }
    else {
        msg = "Are you sure to activate the user";
    }
    bootbox.confirm({
        //title: "Message",
        //message: "Do you want to Change Member Status",
        message: msg,
        buttons: {
            confirm: {
                label: 'Confirm',
                className: 'btn-success btn-sm'
            },
            cancel: {
                label: 'Cancel',
                className: 'btn-danger btn-sm'
            }
        },
        callback: function (result) {
            if (result) {
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    //@*url:"@Url.Action("MemberStatusUpdate", "MemberAPILabel" , new {area = "Admin"})",*@
                    url: "/MemberAPILabel/MemberStatusUpdate?area=Admin",
                    data: {
                        __RequestVerificationToken: token,
                        id: id, statusval: statusval
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {

                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Status changed successfully",
                                backdrop: true
                            });

                        }
                        else {
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                message: "there is some thing error",
                                backdrop: true
                            });
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log(status);
                    }
                });
            }
        }
    });

}
function SendMailToMember(id) {    
    bootbox.confirm({
        //title: "Message",
        message: "Do you want to Send mail for Password to user",
        buttons: {
            confirm: {
                label: 'Confirm',
                className: 'btn-success btn-sm'
            },
            cancel: {
                label: 'Cancel',
                className: 'btn-danger btn-sm'
            }
        },
        callback: function (result) {
            if (result) {
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    //url: "@Url.Action("PasswordSendtoUser", "MemberAPILabel" , new {area="Admin"})",
                    //url: "/Admin/MemberAPILabel/PasswordSendtoUser",
                    url: "/MemberAPILabel/PasswordSendtoUser?area=Admin",
                    data: {
                        __RequestVerificationToken: token,
                        id: id
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {

                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Password send successfully to the registered mail.",
                                backdrop: true
                            });

                        }
                        else {
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                message: "there is some thing error",
                                backdrop: true
                            });
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log(status);
                    }
                });
            }
        }
    });

}

function GetPassword(id) {
    var token = $(':input[name="__RequestVerificationToken"]').val();
    $.ajax({

        url: "/MemberAPILabel/GetMemberPassword?area=Admin",
        data: {
            __RequestVerificationToken: token,
            id: id
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            console.log(data);
            debugger;
            bootbox.alert({
                size: "small",
                message: "This is your password: <b>" + data.User_pwd + "</b>",
                backdrop: true
            });
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}



$('#txtDistributorEmailIdChecking').on('input blur change keyup', function () {
    
    if ($(this).val().length != 0) {
        const MobileNo = $('#txtDistributorMobileNocheck').val();
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberAPILabel/CheckMobileNoEmailAvailability?area=Admin",
            
            data: {
                __RequestVerificationToken: token,
                MobileNo:MobileNo,
                EmailId: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#txtDistributorEmailIdChecking').css('border', '3px #090 solid');
                    $('#btnsubmit').attr('disabled', false);
                }
                else {
                    $('#txtDistributorEmailIdChecking').css('border', '3px #C33 solid');
                    $('#btnsubmit').attr('disabled', true);
                    //alert("This email id is already registered");
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
            }
        });
    }
    else {
        $('#btnsubmit').attr('disabled', true);
    }
});

$('#txtDistributorMobileNocheck').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        const Email = $('#txtDistributorEmailIdChecking').val();
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberAPILabel/CheckMobileNoEmailAvailability?area=Admin",
            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
            data: {
                __RequestVerificationToken: token,
                MobileNo: $(this).val(),
                EmailId: Email
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#txtDistributorMobileNocheck').css('border', '3px #090 solid');
                    $('#btnsubmit').attr('disabled', false);
                }
                else {
                    $('#txtDistributorMobileNocheck').css('border', '3px #C33 solid');
                    $('#btnsubmit').attr('disabled', true);
                    //alert("This email id is already registered");
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
            }
        });
    }
    else {
        $('#btnsubmit').attr('disabled', true);
    }
});

$('#emailaddressDistributor').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberAPILabel/CheckEmailAvailability?area=Admin",
            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
            data: {
                __RequestVerificationToken: token,
                emailid: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#emailaddressDistributor').css('border', '3px #090 solid');
                    $('#btnsubmitDis').attr('disabled', false);
                }
                else {
                    $('#emailaddressDistributor').css('border', '3px #C33 solid');
                    $('#btnsubmitDis').attr('disabled', true);
                    //alert("This email id is already registered");
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
            }
        });
    }
    else {
        $('#btnsubmitDis').attr('disabled', true);
    }
});


$('#emailaddressMerchant').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberAPILabel/CheckEmailAvailability?area=Admin",
            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
            data: {
                __RequestVerificationToken: token,
                emailid: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#emailaddressMerchant').css('border', '3px #090 solid');
                    $('#btnsubmitMer').attr('disabled', false);
                }
                else {
                    $('#emailaddressMerchant').css('border', '3px #C33 solid');
                    $('#btnsubmitMer').attr('disabled', true);
                    //alert("This email id is already registered");
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
            }
        });
    }
    else {
        $('#btnsubmitMer').attr('disabled', true);
    }
});


var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
function ValidateUploadlogoInput(oInput) {
    debugger;
    var FileSize = oInput.files[0].size / 1024 / 1024; // in MB
    if (FileSize < .512) {
        if (oInput.type == "file") {
            var sFileName = oInput.value;
            if (sFileName.length > 0) {
                var blnValid = false;
                for (var j = 0; j < _validFileExtensions.length; j++) {
                    var sCurExtension = _validFileExtensions[j];
                    if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                        blnValid = true;
                        break;
                    }
                }
                if (!blnValid) {
                    var msg_alert = "Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", ");
                    bootbox.alert({
                        size: "small",
                        message: msg_alert,
                        backdrop: true
                    });
                    //alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
                    oInput.value = "";
                    return false;
                }
            }
        }
        return true;
    }
    else {
        bootbox.alert({
            size: "small",
            message: "File Size Less Than 512 KB",
            backdrop: true
        });
        oInput.value = "";
        return false;
    }
}

$(function () {

    $('#txtRDSUserName').on('input blur change keyup', function () {

        if ($(this).val().length != 0) {
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/MemberRailwayMappingSetting/CheckRDSUserId?area=Admin',
                data: {
                    rdsUserId: $(this).val(),
                    __RequestVerificationToken: token
                },
                cache: false,
                type: "POST",
                success: function (data) {
                    debugger;
                    if (data.result == "available") {
                        $('#transactionRDSvalueid').hide();
                        //$('#txtRDSUserName').css('border-bottom', '3px #090 solid');
                        $('#txtRDSUserName').css('border', '3px #090 solid');
                        $('#txterror').hide();
                        $('#btnsubmit').prop('disabled', false);
                    }
                    else {
                        $('#transactionRDSvalueid').show();
                        var UserName = data.UserName;
                        var MobileNo = data.MobileNo;
                        var EmailId = data.EmailID;
                        var CompanyName = data.CompanyName;
                        var Address = data.Address;
                        var CompanyGstNo = data.GSTNo;
                        var Panno = data.PANNo;
                        var RDSID = data.RDSId;
                        var MEM_ID = data.MEM_ID;
                        $('#txterror').show();
                        $('#txtRDSUserName').css('border', '3px #C33 solid');
                        $('#txterror').css('border-bottom', '2px #C33 solid').text("RDS User Id Already Allocated To " + UserName + ". Please choose another RDS User Id");
                        $('#btnsubmit').prop('disabled', true);
                        $('#txtUserName').text(UserName);
                        $('#txtUserMobile').text(MobileNo);
                        $('#txtUserEmail').text(EmailId);
                        $('#txtUserCompanyName').text(CompanyName);
                        $('#txtUserAddress').text(Address);
                        $('#txtCompanyGST').text(CompanyGstNo);
                        $('#txtCompanyPanNo').text(Panno);
                        $('#txtRDSId').text(RDSID);
                        $('#txtUserId').text(MEM_ID);
                        if (CompanyGstNo != null) {
                            $('#divgst').css('border-bottom', '1px #CCC solid');
                        }
                        else {
                            $('#divgst').css('border-bottom', '0px ');
                        }
                        if (Panno != null) {
                            $('#divPan').css('border-bottom', '1px #CCC solid');
                        }
                        else {
                            $('#divPan').css('border-bottom', '0px ');
                        }
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                    alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
                }
            });
        }
        else {
            // $('#btnsubmit').attr('disabled', true);
        }
    });

});



var _validFileExtensionsTest = [".cer", ".CER"];
//var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
function ValidateSingleInput(oInput) {
    var FileSize = oInput.files[0].size / 1024 / 1024; // in MB
    if (FileSize < 1) {
        if (oInput.type == "file") {
            var sFileName = oInput.value;
            if (sFileName.length > 0) {
                var blnValid = false;
                for (var j = 0; j < _validFileExtensionsTest.length; j++) {
                    var sCurExtension = _validFileExtensionsTest[j];
                    if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                        blnValid = true;
                        break;
                    }
                }
                if (!blnValid) {
                    var msg_alert = "Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensionsTest.join(", ");
                    bootbox.alert({
                        size: "small",
                        message: msg_alert,
                        backdrop: true
                    });
                    //alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
                    oInput.value = "";
                    return false;
                }
            }
        }
        return true;
    }
    else {
        bootbox.alert({
            size: "small",
            message: "File Size Less Than 1 MB",
            backdrop: true
        });
        oInput.value = "";
        return false;
    }
}

//var _validFileExtensions = [".cer", ".CER"];
////var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
//function ValidateSingleInput(oInput) {
//    if (oInput.type == "file") {
//        var sFileName = oInput.value;
//        if (sFileName.length > 0) {
//            var blnValid = false;
//            for (var j = 0; j < _validFileExtensions.length; j++) {
//                var sCurExtension = _validFileExtensions[j];
//                if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
//                    blnValid = true;
//                    break;
//                }
//            }
//            if (!blnValid) {
//                var msg_alert = "Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", ");
//                bootbox.alert({
//                    size: "small",
//                    message: msg_alert,
//                    backdrop: true
//                });
//                //alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
//                oInput.value = "";
//                return false;
//            }
//        }
//    }
//    return true;
//}

function ActiveServiceUpdate(memberId, id, $this) {
    bootbox.confirm({
        //title: "Message",
        message: (document.getElementById($this).checked) ? "Do you want to activate service" : "Do you want to De-activate service",
        buttons: {
            confirm: {
                label: 'Confirm',
                className: 'btn-success btn-sm'
            },
            cancel: {
                label: 'Cancel',
                className: 'btn-danger btn-sm'
            }
        },
        callback: function (result) {
            if (result) {
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: '/MemberRailServiceTagging/UpdateActiveService?area=Admin',                    
                    //url: "@Url.Action("UpdateActiveService", "DistributorService", new {area= "Distributor" })",
                    data: {
                        __RequestVerificationToken: token,
                        memberId: memberId,
                        Id: id,
                        isActive: document.getElementById($this).checked
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {
                        debugger;
                        if (data.Result === "true") {
                            //$('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "informaiton update successfully",
                                backdrop: true,
                                callback: function () {
                                    var url = "/Admin/MemberRailwayMappingSetting/Index";
                                    window.location.href = url;
                                }
                            });
                        }
                        else {
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                message: "there is some thing error",
                                backdrop: true,
                                callback: function () {
                                    var url = "/Admin/MemberRailwayMappingSetting/Index";
                                    window.location.href = url;
                                }
                            });
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log(status);
                    }
                });
            }
            else {
                if (document.getElementById($this).checked) {
                    $("#" + $this).prop('checked', false);
                }
                else {
                    $("#" + $this).prop('checked', true);
                }

            }
        }
    });

}

//$("#MemberList").change(function () {
//    var MemberId = $('#MemberList').val();
//    //var url = "/PowerAdminMerchantRailServiceTagging/ServiceDetails?memid=" + MemberId;
//    var url = "/PowerAdminMerchantSearch/Index";    
//    window.location = url;
//    // alert(c);
//});

function getParameterByName(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1]);
}
