
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
                    url: "/Retailer/DeleteInformation?area=Distributor",
                    //url:"@Url.Action("DeleteInformation", "Retailer", new {area = "Distributor"})",
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
                                message: "Informaiton Deleted",
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
                    url: "/Retailer/MemberStatusUpdate?area=Distributor",
                    //url: "@Url.Action("MemberStatusUpdate", "Retailer", new {area= "Distributor" })",
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
                                message: "Status changed",
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
                    url: "/Retailer/PasswordSendtoUser?area=Distributor",
                    //url: "@Url.Action("PasswordSendtoUser", "Retailer", new {area= "Distributor" })",
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

        url: "/Retailer/GetMemberPassword?area=Distributor",
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

$('#txtdistributedAddMerchantEmail').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        const MobileNoval = $('#txtdistributedAddMerchantMob').val();
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/Retailer/CheckMobileNoEmailAvailability?area=Distributor",
            //url: "@Url.Action("CheckEmailAvailability", "Retailer", new {area= "Distributor" })",
            data: {
                __RequestVerificationToken: token,
                MobileNo:MobileNoval,
                EmailId: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE               
                if (data.result == "available") {
                    $('#txtdistributedAddMerchantEmail').css('border', '3px #090 solid');
                    $('#btnDistsubmit').attr('disabled', false);
                }
                else {
                    $('#txtdistributedAddMerchantEmail').css('border', '3px #C33 solid');
                    $('#btnDistsubmit').attr('disabled', true);
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
        $('#btnDistsubmit').attr('disabled', true);
    }
});

$('#txtdistributedAddMerchantMob').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        const EmailIDVal = $('#txtdistributedAddMerchantEmail').val();
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/Retailer/CheckMobileNoEmailAvailability?area=Distributor",
            //url: "@Url.Action("CheckEmailAvailability", "Retailer", new {area= "Distributor" })",
            data: {
                __RequestVerificationToken: token,
                MobileNo: $(this).val(),
                EmailId: EmailIDVal
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE               
                if (data.result == "available") {
                    $('#txtdistributedAddMerchantMob').css('border', '3px #090 solid');
                    $('#btnDistsubmit').attr('disabled', false);
                }
                else {
                    $('#txtdistributedAddMerchantMob').css('border', '3px #C33 solid');
                    $('#btnDistsubmit').attr('disabled', true);
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
        $('#btnDistsubmit').attr('disabled', true);
    }
});


$(document).ready(function () {


$("#txtMemberMerchant_MEM_Name").autocomplete({
    source: function (request, response) {
        //var MEm_Type = $('#txtRoleDetails').val();
        $.ajax({
            url: '/Distributor/Retailer/GetMerchantMemberName',
            data: "{ 'prefix': '" + request.term + "'}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                response($.map(data, function (item) {
                    return item;
                }))
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    },
    focus: function (event, ui) {
        $('#txtMemberMerchant_MEM_Name').val(ui.item.label);
        return true;
    },
    select: function (e, i) {
        $("#hfdMerchant_MEM_ID").val(i.item.val);
        return true;
    },
    minLength: 1
});
});