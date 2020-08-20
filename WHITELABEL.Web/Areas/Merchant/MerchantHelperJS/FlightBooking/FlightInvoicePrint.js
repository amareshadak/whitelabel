//var app = angular.module('FlightBookingInvoicePrintApp', ["angularUtils.directives.dirPagination"])
app.controller('FlightBookingInvoicePrintController', ['FlightServices', '$scope', '$http', '$window', function (FlightServices, $scope, $http, $window) {
    $scope.GetBookedFlightInvoice = function () {
        const service = FlightServices.getFlightBookingInvoice();
        service.then(function (response) {
            debugger;
            const data = response.data;
            $scope.GetTicketList = response.data;;
        });
    }

    $scope.PrintInvoice = function (refid) {
        const data = { refId: refid };
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
        debugger;
        const data = { refId: refid, corelation: corelation };
        const service = FlightServices.getHoldTicketConfirm(data);
        service.then(function (response) {
            debugger;
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

    $scope.getAirDate = function (airDate) {
        if (!airDate) return new Date();
        const dateMomentObject = moment(airDate, "DD/MM/YYYY");
        const dateObject = dateMomentObject.toDate();
        return dateObject;
    }


}]);
//getFlightBookingPrintInvoice