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