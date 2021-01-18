function GetComplainReply(id) {
    debugger;
    $.ajax({

        url: "/MerchantComplainRegister/GetAdminReply?area=Merchant",
        data: {            
            id: id
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            console.log(data);
            const msg = data;
            debugger;
            bootbox.alert({
                size: "small",
                message: msg,
                backdrop: true
            });
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}
PostComplainData = function (compainType, compainDetails,ComplintId) {
    debugger;
    if (compainType == null && compainDetails == null) {
        bootbox.alert({
            size: "small",
            message: "Please Fill the Complain Type and Complain Details",
            backdrop: true
        });
        return false;
    }
    $.ajax({
        url: '/MerchantComplainRegister/PostComplainRegister?area=Merchant',
        method: "POST",
        data: { 'ComplainType': compainType, 'ComplainDetails': compainDetails, 'CompId': ComplintId }
    }).then(function (response) {
        var msgdetails = response;
        if (msgdetails == "Complain send to admin") {

            bootbox.alert({
                message: msgdetails,
                size: 'small',
                centerVertical: true,
                backdrop: true,
                callback: function () {
                    console.log(msgdetails);
                    var url = "/Merchant/MerchantComplainRegister/Index";
                    window.location.href = url;
                }
            });
            $("#ComplainRegisterID").modal('hide');
            $(".ComplainRegister").modal('hide');
        }
        else {
            bootbox.alert({
                size: "small",
                message: "Please tray again later",
                backdrop: true
            });
            $("#ComplainRegisterID").modal('hide');
            $(".ComplainRegister").modal('hide');
        }

        console.log(response.data);
    },
        function (response) {
            bootbox.alert({
                size: "small",
                message: "Please tray again later",
                backdrop: true
            });
            console.log(response.data);
        });
};


function EDITonComplain(id) {
    $('#txtCompainTransactioId').val('');    
    $.ajax({        
        url: "/MerchantComplainRegister/GetEditonComplein?area=Merchant",
        data: {
            id: id
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            //$("#ComplainRegisterID").modal('show');
            //$(".ComplainRegister").modal('show');
            console.log(data);
            const msg = data;
            const ComplainID = data.SLN;
            const ComplainType = data.COMPLAIN_TYPE;
            const ComplainDetails = data.COMPLAIN_DETAILS;
            $('#COmplainType').val(ComplainType);
            $('#txtCompainTransactioId').val(ComplainDetails);
            $('#lblsln').val(ComplainID);

        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}
PostReplyComplainData = function (compainID, ReplyDetails) {
    if (compainID == null && ReplyDetails == null) {
        bootbox.alert({
            size: "small",
            message: "Please give the Reply on this complain",
            backdrop: true
        });
        return false;
    }
    $.ajax({
        url: "/MemberComplainList/PostReplyComplainRegister?area=Admin",
        method: "POST",
        data: { 'ComplainId': compainID, 'ReplyDetails': ReplyDetails }
    }).then(function (response) {
        var msgdetails = response;
        bootbox.alert({
            size: "small",
            message: msgdetails,
            backdrop: true
        });
        $("#ComplainRegisterID").modal('hide');
        $(".ComplainRegister").modal('hide');
        console.log(response.data);
    },
        function (response) {
            bootbox.alert({
                size: "small",
                message: "Please tray again later",
                backdrop: true
            });
            console.log(response.data);
        });
};