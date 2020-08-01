
//import { debug } from "util";

var app = angular.module('BulkMerchantCommissionSlabTagging', ["angularUtils.directives.dirPagination"])

//app.filter('offset', function () {
//    return function (input, start) {
//        start = parseInt(start, 2);
//        return input.slice(start);
//    };
//});

app.controller('DistributorTagCommissionSlab', function ($scope, $http, $window, $location) {
    $scope.maxSize = 5;     // limit number for pagination display number.  
    $scope.totalCount = 0;  // total number of items in all pages. initialize as a zero 
    $scope.pageIndex = 1;   // current page number. first page is 1
    $scope.pageSizeSelected = 5; // maximum number of items per page. 

    $scope.WHITELEVELNAME1 = null;
    $scope.CheckId = false;
    $scope.selectAll = false;
    $scope.MobileRechargeSlabdetails = null;
    $scope.UtilityRechargeSlabdetails = null;
    $scope.DMRRechargeSlabdetails = null;
    $scope.AIRSlabdetailsList = null;
    $scope.BusSlabdetailsList = null;
    $scope.HotelSlabdetailsList = null;
    $scope.CashCardSlabdetailsList = null;
    $scope.INTRODUCE_TO_ID = null;
    $scope.INTRODUCER_ID = null;
    GETWhiteLevelName();
    GetMobilerechargeSlab();
    GetUtilityRecharge();
    GetDMRRecharge();
    GetAIRSlabDetails();
    GetBusSlabDetails();
    GetHotelSlabDetails();
    GetCashCardSlabDetails();

    function GETWhiteLevelName() {
        $http({
            url: '/DistributorCommissionTag/AutoComplete/',
            method: "POST",
        }).then(function (response) {
            $scope.WHITELEVELNAME = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    }
    function GetMobilerechargeSlab() {
        $http({
            url: '/DistributorCommissionTag/MobileRechargeSlab/',
            method: "POST",
        }).then(function (response) {
            $scope.MobileRecharge = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetUtilityRecharge() {
        $http({
            url: '/DistributorCommissionTag/UtilityRechargeSlab/',
            method: "POST",
        }).then(function (response) {
            $scope.UtilityRecharge = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetDMRRecharge() {
        $http({
            url: '/DistributorCommissionTag/DMRRechargeSlab/',
            method: "POST",
        }).then(function (response) {
            $scope.DMRRecharge = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetAIRSlabDetails() {
        $http({
            url: '/DistributorCommissionTag/AirSlabDetails/',
            method: "POST",
        }).then(function (response) {
            $scope.AIRSlabDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetBusSlabDetails() {
        $http({
            url: '/DistributorCommissionTag/BusSlabDetails/',
            method: "POST",
        }).then(function (response) {
            $scope.BusSlabDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetHotelSlabDetails() {
        $http({
            url: '/DistributorCommissionTag/HotelSlabDetails/',
            method: "POST",
        }).then(function (response) {
            $scope.HotelSlabDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetCashCardSlabDetails() {
        $http({
            url: '/DistributorCommissionTag/CashCardSlabDetails/',
            method: "POST",
        }).then(function (response) {
            $scope.CashCardSlabDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    $scope.FetchAllMerchant = function () {        
        $http({
            url: '/DistributorCommissionTag/GetAllMerchantListInformation',
            method: "POST",
            data: { 'Mem_Id': $scope.WHITELEVELNAME1 },
        }).then(function (response) {
            $scope.ServiceInformation = response.data;
        },
            function (response) {
                console.log(response.data);
            });
    };

    $scope.ToggleSelectAll = function () {
        $scope.selectAll = !$scope.selectAll;
        angular.forEach(
            $scope.CheckId, function (item) {
                item.selected = $scope.selectAll;
            });
    }
});