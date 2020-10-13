function GetMemberPassword(id) {
    debugger;
    var token = $(':input[name="__RequestVerificationToken"]').val();
    $.ajax({

        url: "/MemberTotalDistributorList/GetMemberPasswordDetails?area=Admin",
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

function GetMemberStatus(id, statusval) {
    var statuschk = statusval;
    var msg = "";
    if (statuschk == "True") {
        msg = "Are you sure to deactivate the user";
    }
    else {
        msg = "Are you sure to activate the user";
    }
    bootbox.confirm({
        title: "Message",
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
                    url: "/MemberTotalDistributorList/MemberGetStatusUpdate?area=Admin",
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


function FetchPaymentGatewayTnx(Sln_Val) {
    $.ajax({

        url: "/MemberRequisitionReport/fetchPaymentGatewayTransaction?area=Admin",
        data: {

            SlnValue: Sln_Val
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            const Res_Data = data.Result;
            const Res_Status = data.Status;
            if (Res_Status == "0") {
                const Txnrefno = Res_Data.PAY_REF_NO;
                const TRANSACTION_AMOUNT = Res_Data.TRANSACTION_AMOUNT;
                const RES_CODE = Res_Data.RES_CODE;
                const RES_STATUS = Res_Data.RES_STATUS;
                const RES_DATE = Res_Data.RES_DATE;
                const SLn = Res_Data.SLN;
                const MEMID = Res_Data.MEM_ID;

                const MyDate_String_Value = RES_DATE;
                const value_Date = new Date
                            (
                                 parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                            );
                var dateTxn = value_Date.getMonth() +
                                         1 +
                                       "/" +
                           value_Date.getDate() +
                                       "/" +
                       value_Date.getFullYear();

                $('#txtPAY_REF_NO').val(Txnrefno);
                $('#txtTRANSACTION_AMOUNT').val(TRANSACTION_AMOUNT);
                $('#txtRES_CODE').val(RES_CODE);
                $('#txtRES_STATUS').val(RES_STATUS);
                $('#txtRES_DATE').val(dateTxn);
                $('#hdn_BillDeskId').val(SLn);
                $('#hdn_BillDeskMEM_ID').val(MEMID);
            }
            else {
                bootbox.alert({
                    size: "small",
                    message: Res_Data,
                    backdrop: true
                });
            }
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}
function FetchFlightMarkupTagging(Sln_Val) {
    $.ajax({

        url: "/MemberFlightMarkupSetting/FetchFlightMarkUpByMember?area=Admin",
        data: {

            SlnValue: Sln_Val
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            const Res_Data = data.Result;
            const Res_Status = data.Status;
            if (Res_Status == "0") {
                const txtMEMBER_UNIQUE_ID = Res_Data.MEMBER_UNIQUE_ID;
                const MEMBER_NAME = Res_Data.MEMBER_NAME;
                const INTERNATIONAL_MARKUP = Res_Data.INTERNATIONAL_MARKUP;
                const DOMESTIC_MARKUP = Res_Data.DOMESTIC_MARKUP;
                const ASSIGN_DATE = Res_Data.ASSIGN_DATE;
                const SLn = Res_Data.SLN;
                const MEMID = Res_Data.MEM_ID;
                const MyDate_String_Value = ASSIGN_DATE;
                const value_Date = new Date
                            (
                                 parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                            );
                var dateTxn = value_Date.getMonth() +
                                         1 +
                                       "/" +
                           value_Date.getDate() +
                                       "/" +
                       value_Date.getFullYear();
                $('#txtMEMBER_UNIQUE_ID').val(txtMEMBER_UNIQUE_ID);
                $('#txtMEMBER_NAME').val(MEMBER_NAME);
                $('#txtInternationalMarkup').val(INTERNATIONAL_MARKUP);
                $('#txtDOMESTIC_MARKUP').val(DOMESTIC_MARKUP);
                //$('#txtRES_DATE').val(dateTxn);
                $('#hdn_MemberId').val(SLn);
                $('#hdn_MEM_ID').val(MEMID);
            }
            else {
                bootbox.alert({
                    size: "small",
                    message: Res_Data,
                    backdrop: true
                });
            }
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}
