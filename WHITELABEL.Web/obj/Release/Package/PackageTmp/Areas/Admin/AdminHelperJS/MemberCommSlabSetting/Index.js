﻿
var app = angular.module('MemberCommissionSlabDistributionApp', [])
app.controller('MemberCommissionSlabDistribution', function ($scope, $http, $window,$location) {
    $scope.SLN = 0;
    $scope.IsVisible = false;
    //$scope.SERVICE_NAME = null;
    //$scope.ID = null;
    //$scope.TYPE = null;
    //$scope.SERVICE_KEY = null;
    //$scope.HSN_SAC = null;
    //$scope.BILLING_MODEL = null;
    //$scope.CommissionPercentage = 0;  
    $scope.buttondisplay = true;
    $scope.SLAB_NAME = null;
    $scope.SLAB_DETAILS = null;
    $scope.SLAB_TYPE = 0;
    $scope.SLAB_STATUS = null;
    $scope.ServiceType = null;
    $scope.OperaotrType = {
        ID: 0,
        SERVICE_NAME: '',
        TYPE: '',
        SERVICE_KEY: '',
        HSN_SAC: '',
        BILLING_MODEL: '',
        COMM_TYPE:null,
        CommissionPercentage: "0",
        DMRFrom: 0,
        DMRTo: 0,
        DMT_TYPE:null
    };
    $scope.ServiceInformationDMR = [
        {
            ID: 1,
            SERVICE_NAME: 'Money Transfer (Domestic)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'DMI',           
            COMM_TYPE: "FIXED",
            CommissionPercentage: "0",
            DMRFrom: 1,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        },
        {
            ID:2,
            SERVICE_NAME: 'Money Transfer (Domestic)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'DMI',
            COMM_TYPE: "FIXED",
            CommissionPercentage: "0",
            DMRFrom: 0,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        },
        {
            ID: 3,
            SERVICE_NAME: 'Money Transfer (Domestic)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'DMI',
            COMM_TYPE: "FIXED",
            CommissionPercentage: "0",
            DMRFrom: 0,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        }
    ]; 

    $scope.ServiceInformationInternational = [
        {
            ID: 1,
            SERVICE_NAME: 'Money Transfer (Nepal)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'PMT',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 1,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        },
        {
            ID: 2,
            SERVICE_NAME: 'Money Transfer (Nepal)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'PMT',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 0,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        },
        {
            ID: 3,
            SERVICE_NAME: 'Money Transfer (Nepal)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'PMT',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 0,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        }
    ];
    $scope.updateDMRFromAmount = function (index) {
        $scope.ServiceInformationDMR[index + 1].DMRFrom = $scope.ServiceInformationDMR[index].DMRTo + 1;
    };
    $scope.updateDMRInternationalAmount = function (index) {
        debugger;
        $scope.ServiceInformationInternational[index + 1].DMRFrom = $scope.ServiceInformationInternational[index].DMRTo + 1;
    };


    $scope.FetchOperator = function () {
        $scope.buttondisplay = true;
        var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';        
        $http({
            url: '/MemberCommissionSlab/GetServiceProvider/',
            method: "POST",
            data: { 'NewListId': $scope.ServiceType }
        }).then(function (response) {            
            if (response.data.length > 0)
            { $scope.buttondisplay = false; }   
            if ($scope.ServiceType === 'DMR') {
                $scope.OperatorDetails = response.data.OperatorDetails;
                $scope.ServiceInformationDMR = response.data.ServiceInformationDMR;
                 $scope.buttondisplay = false; 
            }
            else
            {
                $scope.ServiceInformation = response.data;
            }
            
            console.log(response.data);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    $scope.SaveData = function () {           
        
            var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';
            var SLAB_NAME = $scope.SLAB_NAME;
            var SLAB_DETAILS = $scope.SLAB_DETAILS;
            var ServiceType = $scope.ServiceType;
            //var SLAB_STATUS = $scope.SLAB_STATUS;
            var SLAB_STATUS = $scope.SLAB_STATUS == "Active" ? 1 : 0; 
            var SLAB_TYPE = $scope.SLAB_TYPE;
            var operatortype = document.getElementById("hftServiceType").value;
            if (SLAB_NAME === null || SLAB_DETAILS === null || ServiceType === null)
            {
                return false;
            }
            $('#WL_progress').show();
            if ($scope.ServiceType === 'DMR') {
                OperaotrType = $scope.OperatorDetails;
            }
            else { OperaotrType = $scope.ServiceInformation;
            }
           
            var data = {
                SLAB_NAME: SLAB_NAME,
                SLAB_DETAILS: SLAB_DETAILS,
                SLAB_TYPE: operatortype,
                Slab_TypeName: ServiceType,
                SLAB_STATUS: SLAB_STATUS,
                OperatorDetails: OperaotrType,
                //OperatorDetails: $scope.ServiceInformation[0].TYPE == "REMITTANCE" ? $scope.ServiceInformationDMR : OperaotrType,
                ServiceInformationDMR: $scope.ServiceInformationInternational
            };
            $http({
                url: '/Admin/MemberCommissionSlab/AddCommissionSlab',
                method: "POST",
                data: data,
            }).then(function (response) {
                $scope.ServiceInformation = response.data;
                $('#WL_progress').hide();   
                if (response.data.Result === "Success") {
                    bootbox.alert({
                        message: "Commission Inserted Successfully..",
                        callback: function () {
                            var URL = "/Admin/MemberCommissionSlab/Index/";
                            $window.location.href = URL;
                            console.log('This was logged in the callback!');
                        }
                    });
                    //alert("Commission Inserted Successfully..");
                    //var URL = "/Admin/MemberCommissionSlab/Index/";
                    //$window.location.href = URL;                    
                }
                else if (response.data.Result === "Updated") {
                    bootbox.alert({
                        message: "Commission Updated Successfully..",
                        callback: function () {
                            var URL = "/Admin/MemberCommissionSlab/Index/";
                            $window.location.href = URL;
                            console.log('This was logged in the callback!');
                        }
                    });
                    //alert("Commission Update Successfully..");
                    //var URL = "/Admin/MemberCommissionSlab/Index/";                    
                    //$window.location.href = URL;                    
                }
                else if (response.data.Result === "Failure")
                {
                    bootbox.alert({
                        message: "Commission Percentage is not greater than predefined commission percentage.",
                        callback: function () {
                            var URL = "/Admin/MemberCommissionSlab/Index/";
                            $window.location.href = URL;
                            console.log('This was logged in the callback!');
                        }
                    });
                    //alert("Commission Percentage is not greater than predefined commission percentage.");
                }
                else {
                    //alert("Error Occured");
                    bootbox.alert({
                        message: "Error Occured.",
                        callback: function () {
                            var URL = "/Admin/MemberCommissionSlab/Index/";
                            $window.location.href = URL;
                            console.log('This was logged in the callback!');
                        }
                    });
                }
                console.log(response);
                // success   
            },
                function (response) {
                    console.log(response.data);
                    // optional
                    // failed
                });
    };

    $scope.OnLoadDataBind = function () {
        //debugger;
        var url = "/MemberCommissionSlab/FetchData?idval=" + $scope.SLN;
        $http.get(url)
            .then(function (response) {
                console.log(response);
                $scope.SLN = response.data[0].SLN;
                $scope.SLAB_NAME = response.data[0].SLAB_NAME;
                $scope.SLAB_DETAILS = response.data[0].SLAB_DETAILS;
                $scope.SLAB_TYPE = response.data[0].SLAB_TYPE;
                //$scope.SLAB_STATUS = response.data[0].SLAB_STATUS;
                $scope.SLAB_STATUS = response.data[0].SLAB_STATUS ? "Active" : "Inactive";
                $scope.ServiceType = response.data[0].Slab_TypeName;
                
                //$scope.ServiceInformation = response.data[0].OperatorDetails;
                //$scope.ServiceInformationDMR = response.data[0].OperatorDetails;
                //$scope.ServiceInformationInternational = response.data[0].ServiceInformationDMR;
                if ($scope.ServiceType === 'DMR') {
                    $scope.OperatorDetails = response.data[0].OperatorDetails;
                    $scope.ServiceInformationDMR = response.data[0].ServiceInformationDMR;
                }
                else {
                    $scope.ServiceInformation = response.data[0].OperatorDetails;
                }
            });





        //debugger;
        ////alert($scope.SLN);
        //var ServiceName = '{idval: "' + $scope.SLN + '" }';
        //$http({
        //    url: '/MemberCommissionSlab/FetchData',
        //    method: "POST",
        //    headers: {
        //        'Content-type': 'application/json'
        //    },
        //    data: { 'idval': $scope.SLN }
        //}).then(function (response) {
        //    debugger;
        //    console.log(response);
        //    $scope.SLN = response.data[0].SLN;
        //    //alert(response.data[0].SLAB_NAME);
        //    $scope.SLAB_NAME = response.data[0].SLAB_NAME;
        //    $scope.SLAB_DETAILS = response.data[0].SLAB_DETAILS;
        //    $scope.SLAB_TYPE = response.data[0].SLAB_TYPE;
        //    //alert(response.data[0].SLAB_STATUS);
        //    $scope.SLAB_STATUS = response.data[0].SLAB_STATUS;
        //    $scope.ServiceType = response.data[0].Slab_TypeName;
        //    alert(JSON.stringify(response.data[0].OperatorDetails));
        //    $scope.ServiceInformation = response.data[0].OperatorDetails;
        //    $scope.ServiceInformationDMR = response.data[0].OperatorDetails;
        //    $scope.ServiceInformationInternational = response.data[0].ServiceInformationDMR;

        //    alert(response.data[0].OperatorDetails[1].CommissionPercentage);
           
        //    // success   
        //},
        //    function (response) {
        //        console.log(response.data);
        //        // optional
        //        // failed
        //    });
    };

    $scope.updateValue = function (value) {
        return value;
    }

    $scope.EditValue = function (oldVal, index) {
        var amt = $scope.ServiceInformation[index].CommissionPercentage;
        if (oldVal < amt) {
            $scope.ServiceInformation[index].CommissionPercentage = oldVal;
        }               
    };
    $scope.DMRDomesticEditValue = function (oldVal, index) {
        
        var amt = $scope.OperatorDetails[index].CommissionPercentage;
        if (oldVal < Math.floor(amt)) {
            $scope.OperatorDetails[index].CommissionPercentage = oldVal;
        }
    };
    $scope.DMRInternationalEditValue = function (oldVal, index) {
        
        var amt = $scope.ServiceInformationDMR[index].CommissionPercentage;
        if (oldVal < Math.floor(amt)) {
            $scope.ServiceInformationDMR[index].CommissionPercentage = oldVal;
        }
    };


});

