app.service("FlightServices", ['$http', function ($http) {

    this.getFlightVerificationDetails = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/GetFlightVerificationDetails"
        });
        return request;
    };
    


}]);