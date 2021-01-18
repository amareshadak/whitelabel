function ReplyonComplain(id) {
    $('#txtReplyOnCompain').val('');
    $.ajax({

        url: "/MemberComplainList/GetComplainDetails?area=Admin",
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
            console.log(data);
            const msg = data;
            const ComplainID = data.SLN;
            const ComplainType = data.COMPLAIN_TYPE;
            const ComplainDetails = data.COMPLAIN_DETAILS;
            const ReplyDetails = data.REPLY_DETAILS;
            $('#lblComplainType').html(ComplainType);
            $('#lblCompainTransactioId').html(ComplainDetails);
            $('#lblsln').val(ComplainID);
            $('#txtReplyOnCompain').val(ReplyDetails);

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
        $("#AdminComplainRegisterID").modal('hide');
        $(".AdminComplainRegister").modal('hide');
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