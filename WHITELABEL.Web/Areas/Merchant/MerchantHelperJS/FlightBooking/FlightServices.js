app.service("FlightServices", ['$http', function ($http) {

    this.getFlightSingleSearchDetails = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/SerachFlight"
        });
        return request;
    };

    this.getFareRules = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/GetFareDetails"
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


    this.getRoundTripFlightVerificationDetails = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/GetRoundTripFlightVerificationDetails"
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
    this.getFlightReturnBookeServices = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            
            url: "/Merchant/MerchantFlightDetails/FlightReturnBookingRequest"
        });
        return request;
    }
    this.getFlightReturnHoldBookServices = function (data) {
        var request = $http({
            method: "POST",
            data: data,

            url: "/Merchant/MerchantFlightDetails/FlightReturnHoldBookingRequest"
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
    

    this.getFlightBookingInvoice = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/FlightBookingInvoice"
        });
        return request;
    }
    this.getFlightBookingPrintInvoice = function (data) {        
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/PrintFlightInvoice"
        });
        return request;
    }
    this.getFlightBookingWithoutFareInvoice = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/PrintWithOutFareInvoice"
        });
        return request;
    }
    this.getFlightBookingPublishFareInvoice = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/PrintPublishFareInvoice"
        });
        return request;
    }
    this.getFlightBookingPrintNetFareInvoice = function (data) {
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/PrintNetFareInvoice"
        });
        return request;
    }
    //PrintNetFareInvoice
    this.getFlightBookingInformation = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/TicketInformationGet"
        });
        return request;
    }
    this.getFlightCancellationInformation = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/TIcketCancellationStatusCheck"
        });
        return request;
    }
    this.getHoldTicketConfirm = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/ConfirmHoldTicket"
        });
        return request;
    }
    this.getBooedPassangerList = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/BookedTicketPassangerList"
        });
        return request;
    }
    this.ResheduleBookedTicket = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/ResheduleBookedTicket"
        });
        return request;
    }
    this.GetFligfhtBookingDetails = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/BookedTicketDeatils"
        });
        return request;
    }

    this.getFullCancellationBooedPassangerList = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/returnFullTicketPassangerList"
        });
        return request;
    }
    this.getCancelFlightTicket = function (data) {
        debugger;
        var request = $http({
            method: "POST",
            data: data,
            url: "/Merchant/MerchantFlightDetails/CancelFlightTicket"
        });
        return request;
    }

}]);