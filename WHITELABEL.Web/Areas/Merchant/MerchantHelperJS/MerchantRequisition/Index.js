﻿
$(function () {

    $('#txtReferenceNumber').on('input blur change keyup', function () {        
        if ($(this).val().length != 0) {
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                //url: "@Url.Action("CheckEmailAvailability")",
                url: '/MerchantRequisition/CheckReferenceNo?area=Merchant',
                data: {
                    referenceno: $(this).val(),
                    __RequestVerificationToken: token
                },
                cache: false,
                type: "POST",
                success: function (data) {
                    // DONE
                    debugger;
                    if (data.result == "available") {
                        $('#txtReferenceNumber').css('border', '3px #090 solid');
                        //$('#btnsubmit').attr('disabled', false);
                    }
                    else {
                        debugger;
                        $('#txtReferenceNumber').css('border', '3px #C33 solid');
                        //$('#txtMemberDomain').val(data.Mem_Name);                    
                        //$('#txtMem_ID').val(data.mem_Id);
                        //document.getElementById('txtMem_ID').value = data.mem_Id;
                        //$('#txtAmount').val(data.amt);
                        //$('#txtRequestDate').val(data.Req_Date);
                        //$('#BankID').val(data.Bankid);
                        //$('#Paymentmethod').val(data.paymethod);
                        //$('#txtTransactiondetails').val(data.Transdetails);
                        //$('#txtBankCharges').val(data.BankCharges);

                        //$('#btnsubmit').attr('disabled', true);
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
            // $('#btnsubmit').attr('disabled', true);
        }
    });

});

function DisplayButton() {

    var DistributorchkYes = document.getElementById("checkRequisitionTypeDist");
    var WhitelabelchkYes = document.getElementById("checkRequisitionTypeAdmin");
    const CreditLimitSet = document.getElementById("CreditLimitManagmentID");
    var Paymentgatewaycheckdiv = document.getElementById("RechargewithPaymentGatewway");
    var MemberType = "";
    if (document.getElementById("checkRequisitionTypeDist").checked) {
        MemberType = "Distributor";
		const reqType = "Distributor";
        $('#hdnRequisitionSendto').val(reqType);
        
    }
    if (document.getElementById("checkRequisitionTypeAdmin").checked) {
        MemberType = "Admin";
		        const reqType = "Admin";
        $('#hdnRequisitionSendto').val(reqType);
        //GetBankListAccordnigtoSelect(MemberType);
    }
    if (DistributorchkYes.checked) {
        $('#div_DIstributor').show();
        $('#div_WhiteLabel').hide();
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').show();
        GetBankListAccordnigtoSelect(MemberType);
    }
    else if (WhitelabelchkYes.checked) {
        $('#div_DIstributor').hide();
        $('#div_WhiteLabel').show();
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').show();
        GetBankListAccordnigtoSelect(MemberType);
    }
    else if (Paymentgatewaycheckdiv.checked) {
        $('#checkbyRequisition').hide();
        $('#Paymentgateway').show();
    }
    else if (CreditLimitSet.checked) {
        $('#div_DIstributor').hide();
        $('#div_WhiteLabel').hide();
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').hide();
        $('#CreditLimitRequisition').show();
        //GetBankListAccordnigtoSelect(MemberType);
    }
    else {

    }

}

function PAymentGatewaydiv() {
    debugger;
    var DistributorchkYes = document.getElementById("checkRequisitionTypeDist");
    var WhitelabelchkYes = document.getElementById("checkRequisitionTypeAdmin");
    const CreditLimitSet = document.getElementById("CreditLimitManagmentID");
    var Paymentgatewaycheckdiv = document.getElementById("RechargewithPaymentGatewway");
    var MemberType = "";
    if (document.getElementById("checkRequisitionTypeDist").checked) {
        MemberType = "Distributor";
const reqType = "Distributor";
        $('#hdnRequisitionSendto').val(reqType);
    }
    if (document.getElementById("checkRequisitionTypeAdmin").checked) {
        MemberType = "Admin";
		const reqType = "Admin";
        $('#hdnRequisitionSendto').val(reqType);
        //GetBankListAccordnigtoSelect(MemberType);
    }
    if (Paymentgatewaycheckdiv.checked) {
        $('#checkbyRequisition').hide();
        $('#Paymentgateway').show();
        $('#CreditLimitRequisition').hide();
		const reqType = "PaymentGateway";
        $('#hdnRequisitionSendto').val(reqType);
    }
    else if (DistributorchkYes.checked) {
        $('#div_DIstributor').show();
        $('#div_WhiteLabel').hide();
        $('#Paymentgateway').hide();
        $('#CreditLimitRequisition').hide();
        $('#checkbyRequisition').show();
        GetBankListAccordnigtoSelect(MemberType);
    }
    else if (WhitelabelchkYes.checked) {
        $('#div_DIstributor').hide();
        $('#div_WhiteLabel').show();
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').show();
        $('#CreditLimitRequisition').hide();
        GetBankListAccordnigtoSelect(MemberType);
    }
    else if (CreditLimitSet.checked) {
        $('#div_DIstributor').hide();
        $('#div_WhiteLabel').hide();
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').hide();
        $('#CreditLimitRequisition').show();
        //GetBankListAccordnigtoSelect(MemberType);
    }
    else {
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').show();
    }
}

function CreditManagment() {
    debugger;
    var DistributorchkYes = document.getElementById("checkRequisitionTypeDist");
    var WhitelabelchkYes = document.getElementById("checkRequisitionTypeAdmin");
    const CreditLimitSet = document.getElementById("CreditLimitManagmentID");
    var Paymentgatewaycheckdiv = document.getElementById("RechargewithPaymentGatewway");
    var MemberType = "";
    if (document.getElementById("checkRequisitionTypeDist").checked) {
        MemberType = "Distributor";
        const reqType = "Distributor";
        $('#hdnRequisitionSendto').val(reqType);
    }
    if (document.getElementById("checkRequisitionTypeAdmin").checked) {
        MemberType = "Admin";
        const reqType = "Admin";
        $('#hdnRequisitionSendto').val(reqType);
        //GetBankListAccordnigtoSelect(MemberType);
    }
    if (Paymentgatewaycheckdiv.checked) {
        $('#checkbyRequisition').hide();
        $('#Paymentgateway').show();
        $('#CreditLimitRequisition').hide();
        const reqType = "PaymentGateway";
        $('#hdnRequisitionSendto').val(reqType);
    }
    else if (DistributorchkYes.checked) {
        $('#div_DIstributor').show();
        $('#div_WhiteLabel').hide();
        $('#Paymentgateway').hide();
        $('#CreditLimitRequisition').hide();
        $('#checkbyRequisition').show();
        GetBankListAccordnigtoSelect(MemberType);
    }
    else if (WhitelabelchkYes.checked) {
        $('#div_DIstributor').hide();
        $('#div_WhiteLabel').show();
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').show();
        $('#CreditLimitRequisition').hide();
        GetBankListAccordnigtoSelect(MemberType);
    }
    else if (CreditLimitSet.checked) {
        $('#div_DIstributor').hide();
        $('#div_WhiteLabel').hide();
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').hide();
        $('#CreditLimitRequisition').show();
        //GetBankListAccordnigtoSelect(MemberType);
    }
    else {
        $('#Paymentgateway').hide();
        $('#checkbyRequisition').show();
    }
}


function GetBankListAccordnigtoSelect(GetIntroducer) {
    $("#BankID").empty();
    $("#BankID").val("--Select Bank--");
    var MemberType = GetIntroducer;
    $.ajax({
        url: "/MerchantRequisition/GetBankListAccordingtoMemberType?area=Merchant",       
        data: { MemberType: MemberType },
        cache: false,
        type: "post",
        datatype: "json",
        beforesend: function () {
        },
        success: function (data) {
            $("#BankID").append('<option selected value="">--Select Bank--</option>');
            $.each(data, function (i, data) {
                $("#BankID").append('<option value="' + data.IDValue + '">' +
                    data.TextValue + '</option>');
            });
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}


//$("#DistributorListID").change(function () {
//    $("#MemberList").empty();
//    $("#MemberList").val("--Select--");
//    $.ajax({
//        type: 'POST',
//        url: '@Url.Action("GetMerchant")',
//        dataType: 'json',
//        data: { Disid: $("#DistributorListID").val() },
//        success: function (states) {
//            $("#MemberList").append('<option selected value="">--Select Merchant--</option>');
//            $.each(states, function (i, state) {
//                $("#MemberList").append('<option value="' + state.IDValue + '">' +
//                    state.TextValue + '</option>');
//            });
//            var SuperList = $('#SuperList').val();
//            var DistributorList = $('#DistributorListID').val();
//            var dropdownval = $('#MemberList').val();
//            var status = $('#TransactionStatus').val();
//            var Date_From = $('#txtFromDate').val();
//            var Date_TO = $('#txtToDate').val();
//            $('.mvc-grid').mvcgrid({
//                //query: 'search=' + this.value,
//                //query: 'Super=' + SuperList + '&Distributor=' + DistributorList + '&search=' + dropdownval + '&Status=' + status,
//                query: 'Super=0&Distributor=' + DistributorList + '&search=' + dropdownval + '&Status=' + status + '&DateFrom=' + Date_From + '&Date_To=' + Date_TO,
//                reload: true,
//                reloadStarted: function () {
//                    $(".divFooterTotalComm").remove();
//                },
//            });
//        },
//        error: function (ex) {
//            $("#MemberList").append('<option selected value="">--Select Merchant--</option>');
//            //alert('Failed to retrieve data.' + ex);
//        }
//    });
//    var SuperList = $('#SuperList').val();
//    var DistributorList = $('#DistributorListID').val();
//    var dropdownval = '';
//    var status = '';
//    var Date_From = $('#txtFromDate').val();
//    var Date_TO = $('#txtToDate').val();
//    $('.mvc-grid').mvcgrid({
//        //query: 'search=' + this.value,
//        //query: 'Super=' + SuperList + '&Distributor=' + DistributorList + '&search=' + dropdownval + '&Status=' + status,
//        query: 'Super=0&Distributor=' + DistributorList + '&search=' + dropdownval + '&Status=' + status + '&DateFrom=' + Date_From + '&Date_To=' + Date_TO,
//        reload: true,
//        reloadStarted: function () {
//            $(".divFooterTotalComm").remove();
//        },
//    });
//    return false;
//})


//$(function () {
//$("#BankID").change(function () {
//    debugger;
//    const bankname = $('#BankID').val();
//    if (bankname == "Cash Deposit-Office") {
//        const trnsmethod = "Cash Deposit";
//        $('#Paymentmethod').val(trnsmethod);
//        $('#BankAccountDetails').val(bankname);
//    }
//    else { $('#BankAccountDetails').val(bankname); }

//});
//});
//$(document).ready(function () {
//    var bankname = $('#BankID').val();
//    $('#BankAccountDetails').val(bankname);
//})
