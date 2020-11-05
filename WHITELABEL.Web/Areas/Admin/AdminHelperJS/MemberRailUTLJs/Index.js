function GetapplicationValue(transid, ApplcantID) {
    //debugger;
    var idval = transid;    
    var AppttID = ApplcantID;   
    $('#ApplicantId').val(AppttID);
    $('#hdn_value').val(idval);
    debugger;

    var idval = transid;
    var AppttID = ApplcantID;
    $.ajax({
        url: '/MemberRailAgentInformation/GetMerchantInformation?area=Admin',        
        data: {
            ApplicatioId: idval,
            CustomerId: AppttID
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            var traninfo = data;
            $('#txtMerchantName').val(traninfo.Member_Name);
            $('#txtMobileno').val(traninfo.Mobile);
            $("#txtEmailId").val(traninfo.EmailId);
            $("#sln").val(traninfo.ID);
            $("#txtAppliedDate").val(traninfo.ApplyDate);
            $("#txtAppliedStatus").val(traninfo.APPLICATION_STATUS);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });


}


//function ApplicationApprove(railId, RailPass, applicationId, Applent_Id) {    
function ApplicationApprove(applicationId, Applent_Id) {  
    debugger;
    if (applicationId != '' && Applent_Id != "") {
        bootbox.confirm({        
        message: "Do you want to approve transaction",
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
                //var TransationStatus = "1";
                //var railId_Value = railId;
                //var railpwd = RailPass;
                var Appl_Id = applicationId;
                var Mem_Id = Applent_Id;
                $.ajax({                    
                    url: '/MemberRailAgentInformation/ApproveRailUtilityApplication?area=Admin',                    
                    data: {                        
                        //railId_Value: railId_Value, railpwd: railpwd, Appl_Id: Appl_Id,Mem_Id:Mem_Id
                        Appl_Id: Appl_Id, Mem_Id: Mem_Id
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {                        
                        var mem_id = data.memid;
                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Application approved successfully",
                                backdrop: true,
                                callback: function () {
                                    var url = "/PowerAdminMerchantSearch/GetMerchantDetails?memid=" + mem_id;
                                    window.location.href = url;
                                }
                            });
                            $('#transactionvalueid').modal('hide');
                            $('.transd').modal('hide');
                           
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
    });}
    else {
        //document.getElementById("divRailId").style.borderColor = "thick solid #0000FF";
        document.getElementById("divRailId").style.borderColor = "lightblue";
        document.getElementById("divPwd").style.borderColor = "thick solid #0000FF";
    }
   
}

function ApplicationDeclined(transid) {
    bootbox.confirm({        
        message: "Do you want to decline application",
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
                var TransationStatus = "0";                
                var slnval = transid;
                
                $.ajax({                    
                    url: '/MemberRailAgentInformation/DeclineRailUtilityApplication?area=Admin',                    
                    data: {
                        slnval: slnval
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
                                message: "Application declined",
                                backdrop: true
                            });
                            $('#modalpopup').modal('hide');
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


