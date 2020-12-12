function GetRAILAGENTInfo(transid,MemberId) {
    var idval = transid;
    var Mem_Id = MemberId;
    $.ajax({
        url: '/MemberRailAgentInformation/GetRAILAGENTInfo?area=Admin',
        
        data: {
            TransId: transid,
            Mem_ID:Mem_Id
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            var traninfo = data;        
            $('#txtWLP_Name').val(data.WLP_NAME);
            $('#hdn_WLP_ID').val(data.WLP_ID);
            $('#txtDIST_Name').val(data.DIST_NAME);
            $('#hdn_DIST_ID').val(data.DIST_ID);
            $('#hdn_MEM_ID').val(data.MEM_ID);
            $('#txtRailAgentId').val(data.RAIL_AGENT_ID);
            $('#txtPG_MAX_Val_Name').val(data.PG_MAX_VALUE);
            $('#txtPG_EQUAL_LESS_2000').val(data.PG_EQUAL_LESS_2000);
            $('#txtPG_EQUAL_GREATER_2000').val(data.PG_EQUAL_GREATER_2000);
            $('#ddlPG_GST_STATUS').val(data.PG_GST_STATUS);
            $('#txtADDITIONAL_CHARGE_MAX_VAL').val(data.ADDITIONAL_CHARGE_MAX_VAL);
            $('#txtADDITIONAL_CHARGE_AC').val(data.ADDITIONAL_CHARGE_AC);
            $('#txtADDITIONAL_CHARGE_NON_AC').val(data.ADDITIONAL_CHARGE_NON_AC);
            $('#ddlADDITIONAL_GST_STATUS').val(data.ADDITIONAL_GST_STATUS);
            $('#hdn_Rail_table_Id').val(data.Rail_table_Id);
            $('#hdn_SLN_Id').val(data.SLN);
            $('#ddlPG_Charges_Apply').val(data.PG_Charges_Apply_Val);
            $('#ddlAdditional_Charges_Apply').val(data.Additional_Charges_Apply_Val);
                            
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}

function FetchRailAgentInformation(transid, MemberId) {
    var idval = transid;
    var Mem_Id = MemberId;
    $.ajax({
        url: '/MemberRailAgentInformation/GetRAILAgentInformation?area=Admin',

        data: {
            TransId: transid,
            Mem_ID: Mem_Id
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            const AgentRailInformation = data.data;
            const AgentRailStatus = data.Status;
            if (AgentRailStatus == "0") {
                const RailUserId = AgentRailInformation.RAIL_USER_ID;
                const TRAVEL_AGENT_NAME = AgentRailInformation.TRAVEL_AGENT_NAME;
                const AGENCY_NAME = AgentRailInformation.AGENCY_NAME;
                const OFFICE_ADDRESS = AgentRailInformation.OFFICE_ADDRESS;
                const RESIDENCE_ADDRESS = AgentRailInformation.RESIDENCE_ADDRESS;
                const EMAIL_ID = AgentRailInformation.EMAIL_ID;
                const MOBILE_NO = AgentRailInformation.MOBILE_NO;
                const OFFICE_PHONE = AgentRailInformation.OFFICE_PHONE;
                const PAN_NO = AgentRailInformation.PAN_NO;
                const DIGITAL_CERTIFICATE_DETAILS = AgentRailInformation.DIGITAL_CERTIFICATE_DETAILS;
                const CERTIFICATE_BEGIN_DATE = AgentRailInformation.CERTIFICATE_BEGIN_DATE;
                const CERTIFICATE_END_DATE = AgentRailInformation.CERTIFICATE_END_DATE;
                const USER_STATE = AgentRailInformation.USER_STATE;
                const AGENT_VERIFIED_STATUS = AgentRailInformation.AGENT_VERIFIED_STATUS;
                const DEACTIVATION_REASON = AgentRailInformation.DEACTIVATION_REASON;
                const AADHAAR_VERIFICATION_STATUS = AgentRailInformation.AADHAAR_VERIFICATION_STATUS;
                const ENTRY_DATE = AgentRailInformation.ENTRY_DATE;
                const STATUS = AgentRailInformation.STATUS;
                const STATE_ID = AgentRailInformation.STATE_ID;
                const RAIL_COMM_TAG = AgentRailInformation.RAIL_COMM_TAG;
                $('#lblRailid').text(RailUserId);
                $('#lblTRAVEL_AGENT_NAME').text(TRAVEL_AGENT_NAME);
                $('#lblAGENCY_NAME').text(AGENCY_NAME);
                $('#lblOFFICE_ADDRESS').text(OFFICE_ADDRESS);
                $('#lblEMAIL_ID').text(EMAIL_ID);
                $('#lblMOBILE_NO').text(MOBILE_NO);
                $('#lblOFFICE_PHONE').text(OFFICE_PHONE);
                $('#lblPAN_NO').text(PAN_NO);
                $('#lblAadhaarcardverification').text(AADHAAR_VERIFICATION_STATUS);
                $('#lblAGENT_VERIFIED_STATUS').text(AGENT_VERIFIED_STATUS);
                $('#lblRESIDENCE_ADDRESS').text(RESIDENCE_ADDRESS);
                $('#lblRESIDENCE_ADDRESS').text(RESIDENCE_ADDRESS);
                $('#lblRESIDENCE_ADDRESS').text(RESIDENCE_ADDRESS);
            }
            else {
                bootbox.alert({
                    size: "small",
                    message: "Rail agent information is not available.Please enter rail agnet.",
                    backdrop: true,
                    callback: function () {
                    }
                });
            }
           

        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}



//$(document).ready(function () {
//    $("#txtRailMemberaName").autocomplete({
//        source: function (request, response) {
//            $.ajax({
//                url: '/PowerAdminRailAgentInformation/GetMemberName',
//                data: "{ 'prefix': '" + request.term + "'}",
//                dataType: "json",
//                type: "POST",
//                contentType: "application/json; charset=utf-8",
//                success: function (data) {
//                    response($.map(data, function (item) {
//                        return item;
//                    }))
//                },
//                error: function (response) {
//                    alert(response.responseText);
//                },
//                failure: function (response) {
//                    alert(response.responseText);
//                }
//            });
//        },
//        focus: function (event, ui) {
//            $('#txtRailMemberaName').val(ui.item.label);
//            return true;
//        },
//        select: function (e, i) {
//            $("#RailIDMemberId").val(i.item.val);
//            return true;
//        },
//        minLength: 1
//    });
//});