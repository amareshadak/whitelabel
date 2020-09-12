app.controller('FlightVerificationController', ['FlightServices', '$scope', '$http', '$window', function (FlightServices, $scope, $http, $window) {
    
    $scope.additionalAddedAmount = parseFloat(document.getElementById('AIRADDITIONALAMOUNT').value);
    $scope.adult = 0;
    $scope.child = 0;
    $scope.infant = 0;
    $scope.passengers = [];
    $scope.trackNumber = '';
    $scope.segments = [];
    $scope.additionaServices = [];
    $scope.TotalAmount = 0;
    $scope.userMarkup = 0;
    $scope.popoverFeesAndTax = {
        content: '',
        templateUrl: 'templateFeesAndTax.html',
        title: ''
    };


    let pessengerObj = {
        "PaxSeqNo": 0,
        "Title": "",
        "FirstName": "",
        "LastName": "",
        "PassengerType": "",
        "DateOfBirth": "",
        //"PassportNo": "",
        //"PassportExpDate": "",
        //"PassportIssuingCountry": "IND",
        //"NationalityCountry": "IND",
        "label": ""
    };



    $scope.bookingRequestObj = {
        "RequestXml": {
            "Authenticate": {
                "InterfaceCode": "",
                "InterfaceAuthKey": "",
                "AgentCode": "",
                "Password": ""
            },
            "BookTicketRequest": {
                "TrackNo": $scope.trackNumber,
                "MobileNo": "",
                "AltMobileNo": "",
                "Email": "",
                "Address": "",
                "ClientRequestID": "",
                "Passengers": {
                    "Passenger": $scope.passengers
                },
                "Segments": {
                    "Segment": $scope.segments,
                },
                "AdditionalServices": {
                    "AdditionalService": [
                        {
                            "PaxSeqNo": "1",
                            "FromStationCode": "AMD",
                            "ToStationCode": "BLR",
                            "Type": "Meal",
                            "Amount": "275.0000",
                            "ServiceCode": "VGML",
                            "ServiceFlightKey": "20190515-SG-921-AMDBLR"
                        },
                        {
                            "PaxSeqNo": "2",
                            "FromStationCode": "AMD",
                            "ToStationCode": "BLR",
                            "Type": "Meal",
                            "Amount": "275.0000",
                            "ServiceCode": "VGML",
                            "ServiceFlightKey": "20190515-SG-921-AMDBLR"
                        }
                    ]
                },
                "TotalAmount": "",
                "MerchantCode": "PAY9zJhspxq7m",
                "MerchantKey": "eSpbcYMkPoZYFPcE8FnZ",
                "SaltKey": "WHJIIcNjVXaZj03TnDme", 
                "IsTicketing": "Yes"
            }
        }
    };
    
    $scope.additionaServicesObj = {
        "RequestXml": {
            "Authenticate": {
                "InterfaceCode": "",
                "InterfaceAuthKey": "",
                "AgentCode": "",
                "Password": ""
            },
            "GetAdditionalServicesRequest": {
                "TrackNo": ''
            }
        }
    };

    $scope.ServicesMeals = [];
    $scope.ServicesSeats = [];
    $scope.ServicesBaggage = [];

    $scope.totalBaseFare = function (fareDetails) {
        if (fareDetails) {
            return (parseFloat(fareDetails[0].AdultBaseFare) + parseFloat(fareDetails[0].ChildBaseFare) + parseFloat(fareDetails[0].InfantBaseFare));
        }
        return '';
    }
    $scope.totalTaxAndCharges = function (fareDetails) {
        if (fareDetails) {
            return (
                parseFloat(fareDetails[0].AdultTax)
                + parseFloat(fareDetails[0].ChildTax)
                + parseFloat(fareDetails[0].InfantTax)
                + parseFloat(fareDetails[0].AdultCuteFee)
                + parseFloat(fareDetails[0].ChildCuteFee)
                + parseFloat(fareDetails[0].InfantCuteFee)
            );
        }
        return '';
    }

    $scope.totalAmountCalculation = function (amount) {
        if (amount) {
            return parseFloat(amount) + $scope.additionalAddedAmount + parseFloat($scope.userMarkup);
        }
        return 0;
    }

    $scope.loadFlightDetails = function (trackNo, tripMode) {
        const data = { TrackNo: trackNo, TripMode: tripMode };
        const service = FlightServices.getFlightVerificationDetails(data);
        service.then(function (response) {
            debugger;
            const data = response.data;
            const verifyFlightDetailResponse = JSON.parse(data).VerifyFlightDetailResponse;
            const flightDetails = verifyFlightDetailResponse.FlightDetails;
            const fareDetails = verifyFlightDetailResponse.FareDetails;
            $scope.TotalAmount = fareDetails[0].TotalAmount;
            $scope.flightDetails = flightDetails;
            $scope.fareDetails = fareDetails;
            $scope.detailsLoadingError = verifyFlightDetailResponse.Error;
            $scope.trackNumber = $scope.flightDetails[0].TrackNo;

            console.log(verifyFlightDetailResponse)
           


            $scope.additionaServicesObj.RequestXml.GetAdditionalServicesRequest.TrackNo = $scope.flightDetails[0].TrackNo;
            $scope.loadAdditionalServices();
            $scope.loadSegment();

        });
    }

    $scope.loadSegment = function () {
        let SegmentSeqNo = 1;
        angular.forEach($scope.flightDetails, function (value, key) {
            let data = {
                "TrackNo": value.TrackNo,
                "SegmentSeqNo": SegmentSeqNo,
                "AirlineCode": value.AirlineCode,
                "FlightNo": value.FlightNo,
                "FromAirportCode": value.FromAirportCode,
                "ToAirportCode": value.ToAirportCode,
                "DepDate": value.DepDate,
                "DepTime": value.DepTime,
                "ArrDate": value.ArrDate,
                "ArrTime": value.ArrTime,
                "FlightClass": value.FlightClass,
                "MainClass": value.MainClass
            };

            $scope.segments.push(data);

            SegmentSeqNo++;
        });

        
    }

    $scope.loadAdditionalServices = function () {
        const req = { req: JSON.stringify($scope.additionaServicesObj) };
        const service = FlightServices.getAdditionalServices(req);
        service.then(function (response) {
            const data = JSON.parse(response.data).GetAdditionalServicesResponse.SSRResponses[0].SSRResponse;
            let PaxSeqNo = 1;
            angular.forEach(data, function (value, key) {
                let boj = {
                    "PaxSeqNo": PaxSeqNo,
                    "FromStationCode": value.FromStationCode,
                    "ToStationCode": value.ToStationCode,
                    "Type": value.Type,
                    "Amount": value.Amount,
                    "ServiceCode": value.ServiceCode,
                    "ServiceFlightKey": value.ServiceFlightKey,
                    "IsSelected": false
                };

                $scope.additionaServices.push(boj);
                PaxSeqNo++;
            });

            
            $scope.ServicesMeals = data.filter(x => { return x.Type == 'Meal'; });
            $scope.ServicesSeats = data.filter(x => { return x.Type == 'Seat'; });;
            $scope.ServicesBaggages = data.filter(x => { return x.Type == 'Baggage'; });
            $scope.isAddtionalServives = data.length > 0;

        })
    }

    $scope.populatePesengerList = function (adult, child, infant) {
        $scope.passengers = [];
        $scope.adult = adult ? parseInt(adult) : 0;
        if ($scope.adult > 0) {
            for (let i = 1; i <= $scope.adult; i++) {
                let objCopy = Object.assign({}, pessengerObj);
                objCopy.PaxSeqNo = i;
                objCopy.PassengerType = "A";
                objCopy.label = `Adult ${i}`;
                // objCopy.PassportNo = `RTTTTGGBGB5635${i}`,
                $scope.passengers.push(objCopy)
            }
        }
        $scope.child = child ? parseInt(child) : 0;
        if ($scope.child > 0) {
            for (let i = 1; i <= $scope.child; i++) {
                let objCopy = Object.assign({}, pessengerObj);
                objCopy.PaxSeqNo = $scope.passengers.length + i;
                objCopy.PassengerType = "C";
                objCopy.label = `Child ${i}`;
                // objCopy.PassportNo = `RTTTTGGBGB5639${i}`,
                $scope.passengers.push(objCopy)
            }
        }
        $scope.infant = infant ? parseInt(infant) : 0;
        if ($scope.infant > 0) {
            for (let i = 1; i <= $scope.infant; i++) {
                let objCopy = Object.assign({}, pessengerObj);
                objCopy.PaxSeqNo = $scope.passengers.length + i;
                objCopy.PassengerType = "I";
                objCopy.label = `Infant ${i}`;
                // objCopy.PassportNo = `RTTTTGGBGB5637${i}`,
                $scope.passengers.push(objCopy)
            }
        }

    }

    $scope.getAirDate = function (airDate) {
        if (!airDate) return new Date();
        const dateMomentObject = moment(airDate, "DD/MM/YYYY");
        const dateObject = dateMomentObject.toDate();
        return dateObject;
    }

    $scope.calculateDurationTime = function (n) {
        var minutes = n % 60
        var hours = (n - minutes) / 60
        return hours + " hr " + minutes + " m";
    }

    $scope.bookFlightRequest = function () {        
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.TrackNo = $scope.trackNumber;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.Segments.Segment = $scope.segments;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.TotalAmount = $scope.TotalAmount;

        // console.log(JSON.stringify($scope.bookingRequestObj))
        // const reqObj = { "RequestXml": { "Authenticate": { "InterfaceCode": "", "InterfaceAuthKey": "", "AgentCode": "", "Password": "" }, "BookTicketRequest": { "TrackNo": "0$48957|4|27AO", "MobileNo": "9879879846", "AltMobileNo": "9549879849", "Email": "amareshadak@gmail.com", "Address": "", "ClientRequestID": "", "Passengers": { "Passenger": [{ "PaxSeqNo": 1, "Title": "Mr", "FirstName": "Amaresh", "LastName": "Adak", "PassengerType": "A", "DateOfBirth": "10/04/1991", "PassportNo": "RTTTTGGBGB56351", "PassportExpDate": "", "PassportIssuingCountry": "IND", "NationalityCountry": "IND", "label": "Adult 1", "$$hashKey": "object:3" }] }, "Segments": { "Segment": [{ "TrackNo": "0$48957|4|27AO", "SegmentSeqNo": 1, "AirlineCode": "UK", "FlightNo": "720", "FromAirportCode": "CCU", "ToAirportCode": "DEL", "DepDate": "16/08/2020", "DepTime": "07:10", "ArrDate": "16/08/2020", "ArrTime": "09:35", "FlightClass": "Q", "MainClass": "Y" }, { "TrackNo": "0$48957|4|27AO", "SegmentSeqNo": 2, "AirlineCode": "UK", "FlightNo": "1400", "FromAirportCode": "DEL", "ToAirportCode": "BOM", "DepDate": "16/08/2020", "DepTime": "13:00", "ArrDate": "16/08/2020", "ArrTime": "15:10", "FlightClass": "Q", "MainClass": "Y" }] }, "AdditionalServices": { "AdditionalService": [] }, "TotalAmount": "9895", "MerchantCode": "PAY9zJhspxq7m", "MerchantKey": "eSpbcYMkPoZYFPcE8FnZ", "SaltKey": "WHJIIcNjVXaZj03TnDme", "IsTicketing": "Yes" } } };
        // console.log(reqObj);

        const req = { req: JSON.stringify($scope.bookingRequestObj), userMarkup: $scope.userMarkup, FlightAmt: $scope.TotalAmount, TripMode:'O'};
        const service = FlightServices.getFlightBookeServices(req);

        service.then(function (response) {            
            try {
             

                let data = JSON.parse(response.data);
                if (data.BookTicketResponses.BookTicketResponse.length > 0) {
                    //$('#modelTicketConfirmed').modal('show');
                    bootbox.alert({
                        message: "Your booking is confirmed.",
                        callback: function () {
                            //var URL = "/Merchant/MerchantFlightDetails";
                            var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton"
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
            } catch (e) {
                const msgval = response.data;
                if (msgval == "Your wallet balance is insufficient to book a ticket.") {
                    bootbox.alert({
                        message: "Your wallet balance is insufficient to book a ticket.",
                        callback: function () {
                            //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                            //$window.location.href = URL;
                            //console.log('This was logged in the callback!');
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
                //bootbox.alert({
                //    message: "Please check all the information and submit again.",
                //    callback: function () {
                //        //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                //        //$window.location.href = URL;
                //        //console.log('This was logged in the callback!');
                //    }
                //});
            }
           
            
        });

    };


    $scope.holdingFlightRequest = function () {

        $scope.bookingRequestObj.RequestXml.BookTicketRequest.TrackNo = $scope.trackNumber;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.Segments.Segment = $scope.segments;
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
        $scope.bookingRequestObj.RequestXml.BookTicketRequest.TotalAmount = $scope.TotalAmount;
        if ($scope.flightDetails[0].HoldAllowed == 'Y') {
            $scope.bookingRequestObj.RequestXml.BookTicketRequest.HoldAllowed = 'Y';
            $scope.bookingRequestObj.RequestXml.BookTicketRequest.HoldCharge = $scope.flightDetails[0].HoldCharges;
        }
      

        // console.log(JSON.stringify($scope.bookingRequestObj))
        // const reqObj = { "RequestXml": { "Authenticate": { "InterfaceCode": "", "InterfaceAuthKey": "", "AgentCode": "", "Password": "" }, "BookTicketRequest": { "TrackNo": "0$48957|4|27AO", "MobileNo": "9879879846", "AltMobileNo": "9549879849", "Email": "amareshadak@gmail.com", "Address": "", "ClientRequestID": "", "Passengers": { "Passenger": [{ "PaxSeqNo": 1, "Title": "Mr", "FirstName": "Amaresh", "LastName": "Adak", "PassengerType": "A", "DateOfBirth": "10/04/1991", "PassportNo": "RTTTTGGBGB56351", "PassportExpDate": "", "PassportIssuingCountry": "IND", "NationalityCountry": "IND", "label": "Adult 1", "$$hashKey": "object:3" }] }, "Segments": { "Segment": [{ "TrackNo": "0$48957|4|27AO", "SegmentSeqNo": 1, "AirlineCode": "UK", "FlightNo": "720", "FromAirportCode": "CCU", "ToAirportCode": "DEL", "DepDate": "16/08/2020", "DepTime": "07:10", "ArrDate": "16/08/2020", "ArrTime": "09:35", "FlightClass": "Q", "MainClass": "Y" }, { "TrackNo": "0$48957|4|27AO", "SegmentSeqNo": 2, "AirlineCode": "UK", "FlightNo": "1400", "FromAirportCode": "DEL", "ToAirportCode": "BOM", "DepDate": "16/08/2020", "DepTime": "13:00", "ArrDate": "16/08/2020", "ArrTime": "15:10", "FlightClass": "Q", "MainClass": "Y" }] }, "AdditionalServices": { "AdditionalService": [] }, "TotalAmount": "9895", "MerchantCode": "PAY9zJhspxq7m", "MerchantKey": "eSpbcYMkPoZYFPcE8FnZ", "SaltKey": "WHJIIcNjVXaZj03TnDme", "IsTicketing": "Yes" } } };
        // console.log(reqObj);

        const req = { req: JSON.stringify($scope.bookingRequestObj), userMarkup: $scope.userMarkup, FlightAmt: $scope.TotalAmount, TripMode: 'O' };
        const service = FlightServices.getFlightHoldingServices(req);

        service.then(function (response) {
            try {
                let data = JSON.parse(response.data);
                if (data.BookTicketResponses.BookTicketResponse.length > 0) {
                    //$('#modelTicketConfirmed').modal('show');
                    bootbox.alert({
                        message: "Holding confirmed.",
                        callback: function () {
                            //var URL = "/Merchant/MerchantFlightDetails";
                            var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton"
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
            } catch (e) {
                const msgval = response.data;
                if (msgval == "Your wallet balance is insufficient to book a ticket.") {
                    bootbox.alert({
                        message: "Your wallet balance is insufficient to book a ticket.",
                        callback: function () {
                            //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                            //$window.location.href = URL;
                            //console.log('This was logged in the callback!');
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
                //bootbox.alert({
                //    message: "Please check all the information and submit again.",
                //    callback: function () {
                //        //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                //        //$window.location.href = URL;
                //        //console.log('This was logged in the callback!');
                //    }
                //});
            }


        });

    };

    $scope.PrintFlightInvoice = function (ref_no,Pnr)
    {
        debugger;
    };

}]);

