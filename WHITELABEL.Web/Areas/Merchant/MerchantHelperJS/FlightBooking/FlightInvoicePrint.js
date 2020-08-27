//var app = angular.module('FlightBookingInvoicePrintApp', ["angularUtils.directives.dirPagination"])
app.controller('FlightBookingInvoicePrintController', ['FlightServices', '$scope', '$http', '$window',function (FlightServices, $scope, $http, $window) {

    $scope.viewby = 10;
    $scope.totalItems = null;
    $scope.currentPage = 4;
    $scope.itemsPerPage = $scope.viewby;
    $scope.maxSize = 5; //Number of pager buttons to show

    $scope.GetBookedFlightInvoice = function () {
        const service = FlightServices.getFlightBookingInvoice();
        service.then(function (response) {
            debugger;
            const data = response.data;
            $scope.GetTicketList = response.data;
            $scope.totalItems = response.data.length;
        });
    }

    $scope.PrintInvoice = function (refid, Pnr) {
        debugger;
        const data = { refId: refid, PNR: Pnr };
        const service = FlightServices.getFlightBookingPrintInvoice(data);
        service.then(function (response) {
            debugger;
            const data = response.data;
            const FlightResponse = JSON.parse(data);
            $scope.FlightInvoicePrint = FlightResponse;
            console.log($scope.FlightInvoicePrint);
        });
    };

    $scope.ConfirmHoldTicket = function (refid, corelation) {        
        const data = { refId: refid, corelation: corelation };
        const service = FlightServices.getHoldTicketConfirm(data);
        service.then(function (response) {            
            let data = JSON.parse(response.data);
            if (data.HoldBookingConfirmResponses.HoldBookingConfirmResponse.length > 0) {
                //$('#modelTicketConfirmed').modal('show');
                bootbox.alert({
                    message: "Hold booked ticket is confirmed.",
                    callback: function () {
                        var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
            }
            else {
                bootbox.alert({
                    message: "Please check all the information and submit again.",
                    callback: function () {
                        //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                        //$window.location.href = URL;
                        //console.log('This was logged in the callback!');
                    }
                });
            }
            //const data = response.data;
            //const FlightResponse = JSON.parse(data);
            //$scope.FlightInvoicePrint = FlightResponse;
            console.log($scope.FlightInvoicePrint);
        });
    };

    $scope.DisplayPassangerDetails = function (refid, corelation) {
        debugger;
        $scope.BookedReferenceNumber = refid;
        const data = { refId: refid, corelation: corelation };
        const service = FlightServices.getBooedPassangerList(data);
        service.then(function (response) {
            debugger;
            //let data = JSON.parse(response.data);
            $scope.BookedTicketPassangerList = response.data;
            $('#exampleCancelticketModal').modal('show');
            console.log(data);
        });
    };

    //$scope.CancelFlightTicket = function (refid, corelation) {
    //    debugger;
    //    const data = { refId: refid, corelation: corelation };
    //    const service = FlightServices.getCancelFlightTicket(data);
    //    service.then(function (response) {
    //        debugger;
    //        let data = JSON.parse(response.data);
    //        let msgVAl = data.TicketCancelResponse.Error;
    //        if (data.TicketCancelDetails.TicketCancelResponse.length > 0) {
    //            //$('#modelTicketConfirmed').modal('show');
    //            bootbox.alert({
    //                message: "Hold booked ticket is confirmed.",
    //                callback: function () {
    //                    var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton";
    //                    $window.location.href = URL;
    //                    console.log('This was logged in the callback!');
    //                }
    //            });
    //        }
    //        else {
    //            var msg = data.TicketCancelResponse.Error.errorDescription;

    //            bootbox.alert({
    //                message: msgVAl,
    //                callback: function () {
    //                    //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
    //                    //$window.location.href = URL;
    //                    //console.log('This was logged in the callback!');
    //                }
    //            });
    //        }
    //        //const data = response.data;
    //        //const FlightResponse = JSON.parse(data);
    //        //$scope.FlightInvoicePrint = FlightResponse;
    //        console.log($scope.FlightInvoicePrint);
    //    });
    //};

    $scope.lst = [];
    $scope.CheckBoxchange = function (list, active) {
        debugger;
        if (active) {
            $scope.lst.push(list.SLN);
        }
        else {
            $scope.lst.splice($scope.lst.indexOf(list), 1);
        }
    };


    $scope.CancelFlightTicket = function (PnsgVal, refid) {
        debugger;
        if (PnsgVal.length > 0) {
            const data = { PngsVal: PnsgVal, refId: refid };
            const service = FlightServices.getCancelFlightTicket(data);
            service.then(function (response) {
                debugger;
                let data = response.data;
                var msgVAl = data;
                bootbox.alert({
                    message: msgVAl,
                    callback: function () {
                        var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
            });
        }
        else {
            bootbox.alert({
                message: "Please check the passanger which you have to cancel ticket"
            });
        }
        
    };


    $scope.getAirDate = function (airDate) {
        if (!airDate) return new Date();
        const dateMomentObject = moment(airDate, "DD/MM/YYYY");
        const dateObject = dateMomentObject.toDate();
        return dateObject;
    }
    ///////////////// for pagination of a table
   

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log('Page changed to: ' + $scope.currentPage);
    };

    $scope.setItemsPerPage = function (num) {
        $scope.itemsPerPage = num;
        $scope.currentPage = 1; //reset to first page
    }

}]);
//getFlightBookingPrintInvoice