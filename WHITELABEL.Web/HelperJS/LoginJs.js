$(document).ready(function () {
    $('#txtAddNewMemberemailID').on('input blur change keyup', function () {
        if ($(this).val().length != 0) {
            const MobileNo = $('#txtDistributorMobileNocheck').val();
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: "/Login/CheckMobileNoEmailAvailability",
                //url: "@Url.Action("CheckEmailAvailability", "Login")",
                data: {
                    __RequestVerificationToken: token,
                    MobileNo: MobileNo,
                    EmailId: $(this).val()
                },
                cache: false,
                type: "POST",
                success: function (data) {
                    // DONE
                    if (data.result == "available") {
                        $('#txtAddNewMemberemailID').css('border', '3px #090 solid');
                        $('#btnAddMersubmit').attr('disabled', false);
                    }
                    else {
                        $('#txtAddNewMemberemailID').css('border', '3px #C33 solid');
                        $('#btnAddMersubmit').attr('disabled', true);
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
            $('#btnAddMersubmit').attr('disabled', true);
        }
    });
    $('#txtAddNewMemberMobileNo').on('input blur change keyup', function () {
        if ($(this).val().length != 0) {
            const Email = $('#txtDistributorEmailIdChecking').val();
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: "/Login/CheckMobileNoEmailAvailability",
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
                        $('#txtAddNewMemberMobileNo').css('border', '3px #090 solid');
                        $('#btnAddMersubmit').attr('disabled', false);
                    }
                    else {
                        $('#txtAddNewMemberMobileNo').css('border', '3px #C33 solid');
                        $('#btnAddMersubmit').attr('disabled', true);
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
            $('#btnAddMersubmit').attr('disabled', true);
        }
    });
});

$('#txtDistemailaddress').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/Login/CheckEmailAvailability",
            //url: "@Url.Action("CheckEmailAvailability", "Login")",
            data: {
                __RequestVerificationToken: token,
                emailid: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#txtDistemailaddress').css('border', '3px #090 solid');
                    $('#btnDistsubmit').attr('disabled', false);
                }
                else {
                    $('#txtDistemailaddress').css('border', '3px #C33 solid');
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
    $("#txtDistributorMemberaName").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Login/GetDistributorMemberName',
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
            $('#txtDistributorMemberaName').val(ui.item.label);
            return true;
        },
        select: function (e, i) {
            $("#hfDistributorId").val(i.item.val);
            return true;
        },
        minLength: 1
    });
});