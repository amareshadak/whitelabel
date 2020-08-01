$('#txtMerchantemailaddress').on('input blur change keyup', function () {
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
                    $('#txtMerchantemailaddress').css('border', '3px #090 solid');
                    $('#btnMersubmit').attr('disabled', false);
                }
                else {
                    $('#txtMerchantemailaddress').css('border', '3px #C33 solid');
                    $('#btnMersubmit').attr('disabled', true);
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
        $('#btnMersubmit').attr('disabled', true);
    }
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