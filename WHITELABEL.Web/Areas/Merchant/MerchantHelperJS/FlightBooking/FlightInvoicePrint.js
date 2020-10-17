app.controller('FlightBookingInvoicePrintController', ['FlightServices', '$scope', '$http', '$window', '$timeout', function (FlightServices, $scope, $http, $window, $timeout) {
    $scope.fromDate = null;
    $scope.toDate = null;
    $scope.viewby = 10;
    $scope.totalItems = null;
    $scope.currentPage = 4;
    $scope.itemsPerPage = $scope.viewby;
    $scope.maxSize = 5; //Number of pager buttons to show
    $scope.GetTicketList = null;
    $scope.GetBookedFlightInvoice = function () {        
        let  data = {};
        if ($scope.fromDate && $scope.toDate ) {
            data = { fromDate: $scope.fromDate, toDate: $scope.toDate };
        }
        const service = FlightServices.getFlightBookingInvoice(data);
        service.then(function (response) {            
            const data = response.data;
            $scope.GetTicketList = response.data;
            $scope.totalItems = response.data.length;
            $timeout(function () {
                if (document.getElementById('hdnIsShowPrintTicket').value == "Show") {
                    document.getElementsByClassName('btn-print-ticket')[0].click();
                }
            }, 100);
        });
    }
    $scope.fullCancellation = "Full";
    $scope.PrintInvoice = function (refid, Pnr) {        
        const data = { refId: refid, PNR: Pnr };
        const service = FlightServices.getFlightBookingPrintInvoice(data);
        service.then(function (response) {
            debugger;
            const data = response.data.result;
            const AddnlCharge = response.data.AdditionalCharge;
            const Processingam = response.data.ProcessingCharge;
            const CompAddress = response.data.Address;
            const CompName = response.data.CompName;
            const CompEmail = response.data.CompEmail;
            const CompContact = response.data.CompContact;
            const FlightResponse = JSON.parse(data);
            $scope.FlightInvoicePrint = FlightResponse;
            $scope.AdditionalCharge = AddnlCharge;
            $scope.ProcessingCharge = Processingam;
            $scope.CompanyAddress = CompAddress;
            $scope.CompanyName = CompName;
            $scope.CompanyEmail = CompEmail;
            $scope.CompanyContact = CompContact;
            console.log($scope.FlightInvoicePrint);
        });
    };
    $scope.BookedTicketInformationFetch = function (refid, Pnr) {        
        const data = { refId: refid, PNR: Pnr };
        const service = FlightServices.getFlightBookingInformation(data);
        service.then(function (response) {
            debugger;
            const data = response.data.result;
            const AddnlCharge = response.data.AdditionalCharge;
            const Processingam = response.data.ProcessingCharge;
            const CompAddress = response.data.Address;
            const CompName = response.data.CompName;
            const CompEmail = response.data.CompEmail;
            const CompContact = response.data.CompContact;
            const FlightResponse = JSON.parse(data);
            $scope.FlightInvoicePrint = FlightResponse;
            $scope.AdditionalCharge = AddnlCharge;
            $scope.ProcessingCharge = Processingam;
            $scope.CompanyAddress = CompAddress;
            $scope.CompanyName = CompName;
            $scope.CompanyEmail = CompEmail;
            $scope.CompanyContact = CompContact;
            console.log($scope.FlightInvoicePrint);
        });
    };
    $scope.CancellationTIcketStatusCheck = function (refid, Pnr) {
        debugger;
        const data = { refId: refid, PNR: Pnr };
        const service = FlightServices.getFlightCancellationInformation(data);
        service.then(function (response) {
            const data = response.data.result;
            const AddnlCharge = response.data.AdditionalCharge;
            const Processingam = response.data.ProcessingCharge;
            const FlightResponse = JSON.parse(data);
            const CompAddress = response.data.Address;
            const CompName = response.data.CompName;
            const CompEmail = response.data.CompEmail;
            const CompContact = response.data.CompContact;
            $scope.FlightInvoicePrint = FlightResponse;
            $scope.AdditionalCharge = AddnlCharge;
            $scope.ProcessingCharge = Processingam;
            $scope.CompanyAddress = CompAddress;
            $scope.CompanyName = CompName;
            $scope.CompanyEmail = CompEmail;
            $scope.CompanyContact = CompContact;
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
            console.log($scope.FlightInvoicePrint);
        });
    };

    $scope.DisplayPassangerDetails = function (refid, corelation, TicketType, FromAirport, ToAirport) {
        debugger;
        $scope.fullCancellation = "";
        $scope.BookedReferenceNumber = refid;
        const data = { refId: refid, corelation: corelation, TicketType: TicketType, FromAirport: FromAirport, ToAirport: ToAirport };
        const service = FlightServices.getBooedPassangerList(data);
        service.then(function (response) {
            debugger;
            //let data = JSON.parse(response.data);
            $scope.BookedTicketPassangerList = response.data;
            $scope.lst = [];
            $('#exampleCancelticketModal').modal('show');
            console.log(data);
        });
    };
    $scope.returnFullCancellationPnsg = function (refid, corelation, TicketType, FromAirport, ToAirport) {
        debugger;
        $scope.fullCancellation = "Full";
        $scope.BookedReferenceNumber = refid;
        const data = { refId: refid, corelation: corelation, TicketType: TicketType, FromAirport: FromAirport, ToAirport: ToAirport };
        const service = FlightServices.getFullCancellationBooedPassangerList(data);
        service.then(function (response) {
            debugger;
            //let data = JSON.parse(response.data);
            $scope.lst = [];
            $scope.BookedTicketPassangerList = response.data;
            $('#exampleCancelticketModal').modal('show');
            console.log(data);
        });
    };

    $scope.lst = [];
    $scope.CheckBoxchange = function (list, active) {
       
        if (active) {
            $scope.lst.push(list.SLN);
        }
        else {
            $scope.lst.splice($scope.lst.indexOf(list), 1);
        }
    };


    $scope.CancelFlightTicket = function (PnsgVal, refid,type) {
        debugger;
        const cancellationType = type;
        if (PnsgVal.length > 0) {
            const data = { PngsVal: PnsgVal, refId: refid, Cancellation_Type: cancellationType };
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



    $scope.GetRecheduleBookTicket = function (refid, corelation, SLN) {
        debugger;
        
        $scope.BookedReferenceNumber = refid;
        $scope.BookedcorelationNumber = corelation;
        $scope.BookingId = SLN;

        $('#RecheduleBookedTicket').modal('show');
        //const data = { refId: refid, corelation: corelation, SLN: SLN };
        //const service = FlightServices.getBooedPassangerList(data);
        //service.then(function (response) {
        //    debugger;
        //    //let data = JSON.parse(response.data);
        //    $scope.BookedTicketPassangerList = response.data;
        //    $scope.lst = [];
        //    $('#RecheduleBookedTicket').modal('show');
        //    console.log(data);
        //});
    };

    $scope.ResheduleSubmit = function (BookingId, rechedulereason, BookedReferenceNumber, BookedcorelationNumber) {
        debugger;
        const rechedulereasonValue=rechedulereason;
        const BReferenceNumber = BookedReferenceNumber;
        const Bookedcorelation = BookedcorelationNumber;
        const BookingIdVal = BookingId;

        
        const data = { refId: BReferenceNumber, corelation: Bookedcorelation, BookingId: BookingIdVal, RescheduleingText: rechedulereasonValue };
        const service = FlightServices.ResheduleBookedTicket(data);
        service.then(function (response) {
            debugger;
            const msg = response.data;
            bootbox.alert({
                message: msg,
                callback: function () {
                    //$('#RecheduleBookedTicket').modal('hide');
                    var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton";
                    $window.location.href = URL;
                    //console.log('This was logged in the callback!');
                }
            });
           
            console.log(data);
        });
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


}])