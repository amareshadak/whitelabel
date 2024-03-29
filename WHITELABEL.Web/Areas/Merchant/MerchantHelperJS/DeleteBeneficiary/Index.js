﻿function DeActivateBeneficiary(id) {
    bootbox.confirm({
        title: "Message",
        message: "Do you want to delete beneficiary account",
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
                    url: "/MerchantDMRDashboard/DeleteBeneficiary?area=Merchant",
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
                        //debugger;
                        var message = data;
                        if (message.msgcode == "TXN") {
                            bootbox.confirm("<form id='infos' action=''>\
                                <input type='hidden' value='"+ message.remitterid + "' id='remitterID' name='remitterID' />\
                                <input type='hidden' value='"+ message.idval + "' id='idval' name='idval' />\
                                <input type='hidden' value='"+ message.beneficiaryid + "' id='BeneficiaryID' name='BeneficiaryID' />\
                                Enter OTP:<input type='text' id='otp' name='otp' /><br/>\<br/>\
                                <label id='msgval'></label>\
                            </form>", function (result) {
                                    if (result) {
                                        var Ajaxurl = '@Url.Action("BeneficiaryAccountdeleteValidate", "MerchantDMRDashboard", new { area = "Merchant" })';
                                        var data = { remitterID: $('#remitterID').val(), BeneficiaryID: $('#BeneficiaryID').val(), otp: $('#otp').val(), Idval: $('#idval').val() };
                                        $.ajax({
                                            type: 'POST',
                                            url: Ajaxurl,
                                            data: JSON.stringify(data),
                                            contentType: "application/json; charset=utf-8",
                                            dataType: 'json',
                                            success: function (response) {
                                                var message = response;
                                                $('.mvc-grid').mvcgrid('reload');
                                                bootbox.alert({
                                                    message: message,
                                                    size: 'small',
                                                    callback: function () {
                                                        console.log(message);
                                                    }
                                                })
                                            }
                                        });
                                    }
                                });
                        }
                        else {
                            bootbox.alert({
                                message: message.status,
                                size: 'small',
                                callback: function () {
                                    console.log(message);
                                }
                            })
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