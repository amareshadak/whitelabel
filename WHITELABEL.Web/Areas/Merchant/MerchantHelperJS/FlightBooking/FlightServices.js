app.service("FlightServices", ['$http', function ($http) {

    this.getFlightSingleSearchDetails = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/SerachFlight"
        });
        return request;
    };

    this.getFlightReturnSearchDetails = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/SerachFlight"
        });
        return request;
    };

    this.getFlightVerificationDetails = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/GetFlightVerificationDetails"
        });
        return request;
    };

    this.getAdditionalServices = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/GetFlightAdditionalDetails"
        });
        return request;
    }

    this.getFlightBookeServices = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/FlightBookingRequest"
        });
        return request;
    }

    this.getFlightHoldingServices = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/FlightHoldingRequest"
        });
        return request;
    }
    

    this.getFlightBookingInvoice = function () {
        var request = $http({
            method: "POST",           
            url: "/Merchant/MerchantFlightDetails/FlightBookingInvoice"
        });
        return request;
    }
    this.getFlightBookingPrintInvoice = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/PrintFlightInvoice"
        });
        return request;
    }

}]);