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
    $scope.getAirDate = function (airDate) {
        if (!airDate) return new Date();
        const dateMomentObject = moment(airDate, "DD/MM/YYYY");
        const dateObject = dateMomentObject.toDate();
        return dateObject;
    }


}]);
//getFlightBookingPrintInvoice