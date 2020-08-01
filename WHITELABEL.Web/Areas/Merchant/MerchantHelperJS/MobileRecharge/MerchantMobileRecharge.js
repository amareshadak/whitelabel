$(function () {
   
    //$("#txtOperator").autocomplete({
    //    source: function (request, response) {
    //        var OperatorType = "";
    //        if (document.getElementById("PrepaidRecharge1").checked) {
    //            OperatorType = "Prepaid";
    //        }
    //        if (document.getElementById("PostpaidRecharge2").checked) {
    //            OperatorType = "POSTPAID";
    //        }
    //        $.ajax({
    //            url: '/MerchantRechargeService/AutoComplete/',
    //            data: "{ 'prefix': '" + request.term + "','OperatorType':'" + OperatorType + "'}",
    //            dataType: "json",
    //            type: "POST",
    //            contentType: "application/json; charset=utf-8",
    //            success: function (data) {
    //                response($.map(data, function (item) {
    //                    return item;
    //                }))
    //            },
    //            error: function (response) {
    //                alert(response.responseText);
    //            },
    //            failure: function (response) {
    //                alert(response.responseText);
    //            }
    //        });
    //    },
    //    select: function (e, i) {
    //        $("#hfOperator").val(i.item.val);
    //    },
    //    minLength: 1
    //});

    $("#txtOperator").autocomplete({
        source: function (request, response) {
            var OperatorType = "";
            if (document.getElementById("PrepaidRecharge1").checked) {
                OperatorType = "Prepaid";
            }
            if (document.getElementById("PostpaidRecharge2").checked) {
                OperatorType = "POSTPAID";
            }
            $.ajax({
                url: '/MerchantRechargeService/AutoComplete/',
                data: "{ 'prefix': '" + request.term + "','OperatorType':'" + OperatorType + "'}",
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
            // this is for when focus of autocomplete item 
            $('#txtOperator').val(ui.item.label);
            return true;
        },
        select: function (e, i) {
            $("#hfOperator").val(i.item.val);
            return true;
        },
        minLength: 1
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + "<img style='width:40px;height:40px' src='data:image;base64," + item.image + "' /> " + item.label + "</a>")
            .appendTo(ul);
    };
    


    $("#txtCircleName").autocomplete({
        source: function (request, response) {
            
            $.ajax({
                url: '/MerchantRechargeService/CircleName/',
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
        select: function (e, i) {
            $("#hfCircleCode").val(i.item.val);
        },
        minLength: 1
    });

});

$(document).ready(function () {
    $("#PrepaidRecharge1").change(function () {
        //debugger;
        $('#txtOperator').val('');
    });
    $("#PostpaidRecharge2").change(function () {

        $('#txtOperator').val('');
    });
});


function GetPostPaidBillDetails() {
    //GetPostPaidBillDetails = function () {
    $('#PostPaidDivElm').show();
    $('#progressMobile').show();    
    var _accountNo = $('#txtContactNo').val();
    var _mobileNo = $('#txtContactNo').val();
    var _geoLocation = $('#GeolocationMobile').val();
    var Service_Key = $('#hfOperator').val();
    var RechargeAmt = $('#txtRechargeAmt').val();
    $.ajax({
        type: "POST",
        url: "/Merchant/MerchantRechargeService/GetPostPaidBillInformation",
        data: { AccountNo: _accountNo, MobileNo: _mobileNo, GeoLocation: _geoLocation, ServiceKey: Service_Key, Amount: RechargeAmt },
        success: function (data) {            
            var Electricitybill = JSON.parse(data);
            if (Electricitybill != "Invalid outletid") {
                if (Electricitybill != "Invalid amount") {
                    var status = Electricitybill.statuscode;
                    var status = Electricitybill.statuscode;
                    var statusvalue = Electricitybill.status;
                    if (status === "TXN") {
                        $('#btnsubmit').show();
                        //document.getElementById("RechargeAmt").innerHTML = JSON.stringify(Electricitybill.data.dueamount);
                        var amount = Electricitybill.data.dueamount;
                        var Gas_Reff = Electricitybill.data.reference_id;
                        $('#txtPostdueamount').text(amount);
                        $('#txtpostpaidduedate').text(Electricitybill.data.duedate);
                        $('#txtpostpaidbillNo').text(Electricitybill.data.billnumber);
                        $('#txtPostpaidbilldate').text(Electricitybill.data.billdate);
                        $('#txtPostpaidbillperiod').text(Electricitybill.data.billperiod);
                        $('#txtpostpaidcustomerparamsdetails').text(Electricitybill.data.customerparamsdetails);
                        $('#txtPostpaidcustomername').text(Electricitybill.data.customername);
                        $('#hdnPostpaidreference_id').val(Electricitybill.data.reference_id);
                        $('#progressMobile').hide();
                        $('#btnDisplay').hide();
                        $('#btnsubmit').show();
                    }
                    else {
                        bootbox.alert({
                            message: statusvalue,
                            size: 'small',
                            backdrop: true,
                            callback: function () {
                                //var urlPath = "/Merchant/MerchantOutletRegistration/Index";
                                //window.location.href = urlPath;
                                console.log(Electricitybill);
                                $('#progressMobile').hide();
                                $('#PostPaidErrormsg').html(statusvalue);
                                $('#PostPaidDivElm').hide();
                            }
                        })
                        $('#PostPaidErrormsg').html(statusvalue);
                        $('#PostPaidDivElm').hide();
                    }
                }
                else {
                    bootbox.alert({
                        message: Electricitybill,
                        size: 'small',
                        backdrop: true,
                        callback: function () {
                            //var urlPath = "/Merchant/MerchantOutletRegistration/Index";
                            //window.location.href = urlPath;
                            console.log(Electricitybill);
                            $('#progressMobile').hide();
                        }
                    })
                }
                $('#progressMobile').hide();
            }
            else {
                bootbox.alert({
                    message: "Please Create Outlet Id>This Outlet Id is Created from Landline Tab",
                    size: 'small',
                    backdrop: true,
                    callback: function () {
                        //var urlPath = "/Merchant/MerchantOutletRegistration/Index";
                        //window.location.href = urlPath;
                        console.log(Electricitybill);
                        $('#progressMobile').hide();
                    }
                })
            }
            $('#progressMobile').hide();

        }

    })
}