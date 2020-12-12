//$('#emailaddress').on('input blur change keyup', function () {

//    if ($(this).val().length != 0) {
//        var token = $(':input[name="__RequestVerificationToken"]').val();
//        $.ajax({
//            url: "/MemberChannelRegistration/CheckEmailAvailability?area=Admin",
//            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
//            data: {
//                __RequestVerificationToken: token,
//                emailid: $(this).val()
//            },
//            cache: false,
//            type: "POST",
//            success: function (data) {
//                // DONE                        
//                if (data.result == "available") {
//                    $('#emailaddress').css('border', '3px #090 solid');
//                    $('#btnsubmit').attr('disabled', false);
//                }
//                else {
//                    $('#emailaddress').css('border', '3px #C33 solid');
//                    $('#btnsubmit').attr('disabled', true);
//                    //alert("This email id is already registered");
//                }
//            },
//            error: function (xhr, status, error) {
//                console.log(xhr.responseText);
//                alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
//            }
//        });
//    }
//    else {
//        $('#btnsubmit').attr('disabled', true);
//    }
//});


$('#emailaddressDistributor').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberAPILabel/CheckMobileNoEmailAvailability?area=Admin",
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

$(function () {
$('#txtemailaddressMerchant').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        const MobileNoValue = $('#txtMobileNoMerchant').val();
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberChannelRegistration/CheckMobileNoEmailAvailability?area=Admin",
            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
            data: {
                __RequestVerificationToken: token,
                MobileNo:MobileNoValue,
                EmailId: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#txtemailaddressMerchant').css('border', '3px #090 solid');
                    $('#btnsubmitMer').attr('disabled', false);
                }
                else {
                    $('#txtemailaddressMerchant').css('border', '3px #C33 solid');
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

$('#txtMobileNoMerchant').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        const EmailIdVal = $('#txtemailaddressMerchant').val();
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberChannelRegistration/CheckMobileNoEmailAvailability?area=Admin",
            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
            data: {
                __RequestVerificationToken: token,
                MobileNo: $(this).val(),
                EmailId: EmailIdVal
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#txtMobileNoMerchant').css('border', '3px #090 solid');
                    $('#btnsubmitMer').attr('disabled', false);
                }
                else {
                    $('#txtMobileNoMerchant').css('border', '3px #C33 solid');
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
});