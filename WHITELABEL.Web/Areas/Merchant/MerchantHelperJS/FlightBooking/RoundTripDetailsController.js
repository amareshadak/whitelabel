app.controller('RoundTripDetailsController', ['FlightServices', '$scope', '$http', '$window', function (FlightServices, $scope, $http, $window) {
    $scope.additionalAddedAmount = parseFloat(document.getElementById('AIRADDITIONALAMOUNT').value);
    $scope.InternationaladditionalAddedAmount = parseFloat(document.getElementById('INTERAIRADDITIONALAMOUNT').value);

    $scope.adult = 0;
    $scope.child = 0;
    $scope.infant = 0;
    $scope.passengers = [];
    $scope.trackNumber = '';
    $scope.segments = [];
    $scope.additionaServices = [];
    $scope.TotalAmount = 0;
    $scope.userMarkup = 0;

    $scope.ReturnTripDeptFrom = '';
    $scope.ReturnTripDeptTo = '';
    $scope.ReturnTripReturnFrom = '';
    $scope.ReturnTripReturnFrom = '';

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

    $scope.totalBaseFare = 0;
    $scope.totalTaxAndCharges = 0;
    $scope.TotalAmount = 0;
    $scope.totalAmount = 0;


    $scope.setTotalFare = function () {
        if ($scope.deptureFareDetails && $scope.returnFareDetails) {
            $scope.totalBaseFare =
                parseFloat($scope.deptureFareDetails.AdultBaseFare) +
                parseFloat($scope.deptureFareDetails.ChildBaseFare) +
                parseFloat($scope.deptureFareDetails.InfantBaseFare) +
                parseFloat($scope.returnFareDetails.AdultBaseFare) +
                parseFloat($scope.returnFareDetails.ChildBaseFare) +
                parseFloat($scope.returnFareDetails.InfantBaseFare);


            $scope.totalTaxAndCharges =
                parseFloat($scope.deptureFareDetails.AdultTax) +
                parseFloat($scope.deptureFareDetails.ChildTax) +
                parseFloat($scope.deptureFareDetails.InfantTax) +
                parseFloat($scope.deptureFareDetails.AdultFuelCharges) +
                parseFloat($scope.deptureFareDetails.ChildFuelCharges) +
                parseFloat($scope.deptureFareDetails.InfantFuelCharges) +
                parseFloat($scope.deptureFareDetails.AdultPassengerServiceFee) +
                parseFloat($scope.deptureFareDetails.ChildPassengerServiceFee) +
                parseFloat($scope.deptureFareDetails.InfantPassengerServiceFee) +
                parseFloat($scope.deptureFareDetails.AdultTransactionFee) +
                parseFloat($scope.deptureFareDetails.ChildTransactionFee) +
                parseFloat($scope.deptureFareDetails.InfantTransactionFee) +
                parseFloat($scope.deptureFareDetails.AdultServiceCharges) +
                parseFloat($scope.deptureFareDetails.ChildServiceCharges) +
                parseFloat($scope.deptureFareDetails.InfantServiceCharges) +
                parseFloat($scope.deptureFareDetails.AdultAirportTax) +
                parseFloat($scope.deptureFareDetails.ChildAirportTax) +
                parseFloat($scope.deptureFareDetails.InfantAirportTax) +
                parseFloat($scope.deptureFareDetails.AdultAirportDevelopmentFee) +
                parseFloat($scope.deptureFareDetails.AdultCuteFee) +
                parseFloat($scope.deptureFareDetails.AdultConvenienceFee) +
                parseFloat($scope.deptureFareDetails.AdultSkyCafeMeals) +
                parseFloat($scope.deptureFareDetails.ChildAirportDevelopmentFee) +
                parseFloat($scope.deptureFareDetails.ChildCuteFee) +
                parseFloat($scope.deptureFareDetails.ChildConvenienceFee) +
                parseFloat($scope.deptureFareDetails.ChildSkyCafeMeals) +
                parseFloat($scope.deptureFareDetails.InfantAirportDevelopmentFee) +
                parseFloat($scope.deptureFareDetails.InfantCuteFee) +
                parseFloat($scope.deptureFareDetails.InfantConvenienceFee) +
                parseFloat($scope.deptureFareDetails.InfantSkyCafeMeals) +
                parseFloat($scope.returnFareDetails.AdultTax) +
                parseFloat($scope.returnFareDetails.ChildTax) +
                parseFloat($scope.returnFareDetails.InfantTax) +
                parseFloat($scope.returnFareDetails.AdultFuelCharges) +
                parseFloat($scope.returnFareDetails.ChildFuelCharges) +
                parseFloat($scope.returnFareDetails.InfantFuelCharges) +
                parseFloat($scope.returnFareDetails.AdultPassengerServiceFee) +
                parseFloat($scope.returnFareDetails.ChildPassengerServiceFee) +
                parseFloat($scope.returnFareDetails.InfantPassengerServiceFee) +
                parseFloat($scope.returnFareDetails.AdultTransactionFee) +
                parseFloat($scope.returnFareDetails.ChildTransactionFee) +
                parseFloat($scope.returnFareDetails.InfantTransactionFee) +
                parseFloat($scope.returnFareDetails.AdultServiceCharges) +
                parseFloat($scope.returnFareDetails.ChildServiceCharges) +
                parseFloat($scope.returnFareDetails.InfantServiceCharges) +
                parseFloat($scope.returnFareDetails.AdultAirportTax) +
                parseFloat($scope.returnFareDetails.ChildAirportTax) +
                parseFloat($scope.returnFareDetails.InfantAirportTax) +
                parseFloat($scope.returnFareDetails.AdultAirportDevelopmentFee) +
                parseFloat($scope.returnFareDetails.AdultCuteFee) +
                parseFloat($scope.returnFareDetails.AdultConvenienceFee) +
                parseFloat($scope.returnFareDetails.AdultSkyCafeMeals) +
                parseFloat($scope.returnFareDetails.ChildAirportDevelopmentFee) +
                parseFloat($scope.returnFareDetails.ChildCuteFee) +
                parseFloat($scope.returnFareDetails.ChildConvenienceFee) +
                parseFloat($scope.returnFareDetails.ChildSkyCafeMeals) +
                parseFloat($scope.returnFareDetails.InfantAirportDevelopmentFee) +
                parseFloat($scope.returnFareDetails.InfantCuteFee) +
                parseFloat($scope.returnFareDetails.InfantConvenienceFee) +
                parseFloat($scope.returnFareDetails.InfantSkyCafeMeals);




            $scope.totalAmount = $scope.totalBaseFare + $scope.totalTaxAndCharges;
        }
    }



    $scope.loadFlightDetails = function (trackNo, tripMode, OriginCode, DestinationCode) {
        
        const track = trackNo.split(',');
        const outBound = track[0];
        const inBound = track[1];





        const data = { outBoundTrackNo: outBound, inBoundTrackNo: inBound, TripMode: tripMode, OriginCode: OriginCode, DestinationCode: DestinationCode };
        const service = FlightServices.getRoundTripFlightVerificationDetails(data);
        service.then(function (response) {
            
            const data = response.data;
            const outBoundFlight = JSON.parse(data.outBoundData).VerifyFlightDetailResponse.FlightDetails;
            const inBoundFlight = JSON.parse(data.inBoundData).VerifyFlightDetailResponse.FlightDetails;
            const ADDITIONALAMT = data.AdditionalAmount;

            console.log(outBoundFlight);
            console.log(inBoundFlight);

            $scope.deptureFlight = outBoundFlight;// flightDetails.filter(x => x.SrNo.charAt(x.SrNo.length - 1) === 'O');
            $scope.returnFlight = inBoundFlight;// flightDetails.filter(x => x.SrNo.charAt(x.SrNo.length - 1) === 'R');

            debugger;
            
            if ($scope.returnFlight[0].HoldAllowed == 'Y' && $scope.deptureFlight[0].HoldAllowed == 'Y') {
                $scope.flightDetails = 'Y';
            }
            else {
                $scope.flightDetails = 'N';
            }
            const fareDetails = JSON.parse(data.outBoundData).VerifyFlightDetailResponse.FareDetails;
            $scope.deptureFareDetails = JSON.parse(data.outBoundData).VerifyFlightDetailResponse.FareDetails[0];
            $scope.returnFareDetails = JSON.parse(data.inBoundData).VerifyFlightDetailResponse.FareDetails[0];
            $scope.setTotalFare();
           
            $scope.TotalAmount = parseFloat($scope.deptureFareDetails.TotalAmount) + parseFloat($scope.returnFareDetails.TotalAmount);
            $scope.DeptNETAmount = parseFloat($scope.deptureFareDetails.NetAmount);
            $scope.ReturnNETAmount = parseFloat($scope.returnFareDetails.NetAmount);
            $scope.TotalPublishFare = parseFloat($scope.deptureFareDetails.TotalAmount) + parseFloat($scope.returnFareDetails.TotalAmount);;
            //// $scope.flightDetails = flightDetails;
            //// $scope.fareDetails = fareDetails;
            //$scope.detailsLoadingError = verifyFlightDetailResponse.Error;
            $scope.trackNumber = $scope.deptureFlight[0].TrackNo + ',' + $scope.returnFlight[0].TrackNo;




            // $scope.additionaServicesObj.RequestXml.GetAdditionalServicesRequest.TrackNo = $scope.flightDetails[0].TrackNo;
            // $scope.loadAdditionalServices();
            $scope.loadSegment();
            $scope.additionalAddedAmount = ADDITIONALAMT;
        });
    }

    $scope.outBoundSegment = [];
    $scope.inBoundSegment = [];

    $scope.loadSegment = function () {        
        let SegmentSeqNo = 1;
        angular.forEach($scope.returnFlight, function (value, key) {
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
            $scope.outBoundSegment.push(data);
            SegmentSeqNo++;
        });


        SegmentSeqNo = 1;
        angular.forEach($scope.deptureFlight, function (value, key) {
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
            $scope.inBoundSegment.push(data);
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

    $scope.bookFlightRequest = function (isFormInvalid) {
        debugger;
        if (isFormInvalid) {
            return false;
        }
        const DeptNetAmt = $scope.DeptNETAmount;
        const ReturnNetAmt = $scope.ReturnNETAmount;

        let OutSegmentValue = $scope.inBoundSegment;
        let inSegmentValue = $scope.outBoundSegment;
        let DeptFareVal = $scope.deptureFareDetails;
        let DeptDeatil = $scope.deptureFlight;
        let outBoundObj = angular.copy($scope.bookingRequestObj);
        outBoundObj.RequestXml.BookTicketRequest.TrackNo = DeptDeatil[0].TrackNo;
        outBoundObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
        outBoundObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
        outBoundObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        outBoundObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        outBoundObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
        outBoundObj.RequestXml.BookTicketRequest.Segments.Segment = OutSegmentValue;
        outBoundObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
        outBoundObj.RequestXml.BookTicketRequest.TotalAmount = DeptFareVal.TotalAmount;
        const DeptTotalFareAmt=DeptFareVal.TotalAmount;
        let InboundFareDetails = $scope.returnFareDetails;
        let InboundFlightDeatil = $scope.returnFlight;
        let InBoundObj = angular.copy($scope.bookingRequestObj);
        InBoundObj.RequestXml.BookTicketRequest.TrackNo = InboundFlightDeatil[0].TrackNo;
        InBoundObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
        InBoundObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
        InBoundObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        InBoundObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        InBoundObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
        InBoundObj.RequestXml.BookTicketRequest.Segments.Segment = inSegmentValue;
        InBoundObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
        InBoundObj.RequestXml.BookTicketRequest.TotalAmount = InboundFareDetails.TotalAmount;
        const ReturnTotalFareAmt=InboundFareDetails.TotalAmount;

        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.TrackNo = $scope.trackNumber;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.Segments.Segment = $scope.segments;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.TotalAmount = $scope.TotalAmount;


        const req = { Deptreq: JSON.stringify(outBoundObj), Retntreq: JSON.stringify(InBoundObj), userMarkup: $scope.userMarkup, FlightAmt: DeptTotalFareAmt, ReturnFlightAmt: ReturnTotalFareAmt, TripMode: 'R', DEPTNetAmt: DeptNetAmt, RetnNetAmt: ReturnNetAmt, deptSegment: JSON.stringify($scope.deptureFlight), returnSegment: JSON.stringify($scope.returnFlight) };
        const service = FlightServices.getFlightReturnBookeServices(req);

        service.then(function (response) {
            try {                
                let data =response.data;
                const DeptRes = data.result;
                const RetRes = data.ReturnRes;
                //$('#modelTicketConfirmed').modal('show');
                if (DeptRes != '') {
                    bootbox.alert({
                        message: DeptRes + " AND " + RetRes,
                        callback: function () {
                            var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton"
                            $window.location.href = URL;
                            console.log('This was logged in the callback!');
                        }
                    });
                }
                else {
                    bootbox.alert({
                        message: RetRes,
                        callback: function () {
                            //var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton"
                            //$window.location.href = URL;
                            console.log('This was logged in the callback!');
                        }
                    });
                }
                    
                //let data = JSON.parse(response.data);
                //if (data.BookTicketResponses.BookTicketResponse.length > 0) {
                //    //$('#modelTicketConfirmed').modal('show');
                //    bootbox.alert({
                //        message: "Your booking is confirmed.",
                //        callback: function () {
                //            var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton"
                //            $window.location.href = URL;
                //            console.log('This was logged in the callback!');
                //        }
                //    });
                //}
                //else {
                //    bootbox.alert({
                //        message: "Please check all the information and submit again.",
                //        callback: function () {
                //            //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                //            //$window.location.href = URL;
                //            //console.log('This was logged in the callback!');
                //        }
                //    });
                //}
            } catch (e) {
                bootbox.alert({
                    message: "Please check all the information and submit again.",
                    callback: function () {
                        //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                        //$window.location.href = URL;
                        //console.log('This was logged in the callback!');
                    }
                });
            }


        });

    };

    $scope.holdingFlightRequest = function (isFormInvalid) {
        //   debugger;
        if (isFormInvalid) {
            return false;
        }
        const DeptNetAmt = $scope.DeptNETAmount;
        const ReturnNetAmt = $scope.ReturnNETAmount;


        let OutSegmentValue = $scope.inBoundSegment;
        let inSegmentValue = $scope.outBoundSegment;
        let DeptFareVal = $scope.deptureFareDetails;
        let DeptDeatil = $scope.deptureFlight;
        let outBoundObj = angular.copy($scope.bookingRequestObj);
        outBoundObj.RequestXml.BookTicketRequest.TrackNo = DeptDeatil[0].TrackNo;
        outBoundObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
        outBoundObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
        outBoundObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        outBoundObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        outBoundObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
        outBoundObj.RequestXml.BookTicketRequest.Segments.Segment = OutSegmentValue;
        outBoundObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
        outBoundObj.RequestXml.BookTicketRequest.TotalAmount = DeptFareVal.TotalAmount;
        if ($scope.flightDetails[0].HoldAllowed == 'Y') {
            outBoundObj.RequestXml.BookTicketRequest.HoldAllowed = 'Y';
            outBoundObj.RequestXml.BookTicketRequest.HoldCharge = $scope.flightDetails[0].HoldCharges;
        }
        const DeptTotalFareAmt = DeptFareVal.TotalAmount;
        let InboundFareDetails = $scope.returnFareDetails;
        let InboundFlightDeatil = $scope.returnFlight;
        let InBoundObj = angular.copy($scope.bookingRequestObj);
        InBoundObj.RequestXml.BookTicketRequest.TrackNo = InboundFlightDeatil[0].TrackNo;
        InBoundObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
        InBoundObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
        InBoundObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        InBoundObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        InBoundObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
        InBoundObj.RequestXml.BookTicketRequest.Segments.Segment = inSegmentValue;
        InBoundObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
        InBoundObj.RequestXml.BookTicketRequest.TotalAmount = InboundFareDetails.TotalAmount;
        if ($scope.flightDetails[0].HoldAllowed == 'Y') {
            InBoundObj.RequestXml.BookTicketRequest.HoldAllowed = 'Y';
            InBoundObj.RequestXml.BookTicketRequest.HoldCharge = $scope.flightDetails[0].HoldCharges;
        }

        const ReturnTotalFareAmt = InboundFareDetails.TotalAmount;

        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.TrackNo = $scope.trackNumber;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.Segments.Segment = $scope.segments;
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
        //$scope.bookingRequestObj.RequestXml.BookTicketRequest.TotalAmount = $scope.TotalAmount;


        const req = { Deptreq: JSON.stringify(outBoundObj), Retntreq: JSON.stringify(InBoundObj), userMarkup: $scope.userMarkup, FlightAmt: DeptTotalFareAmt, ReturnFlightAmt: ReturnTotalFareAmt, TripMode: 'R', DEPTNetAmt: DeptNetAmt, RetnNetAmt: ReturnNetAmt, deptSegment: JSON.stringify($scope.deptureFlight), returnSegment: JSON.stringify($scope.returnFlight) };
        const service = FlightServices.getFlightReturnHoldBookServices(req);

        service.then(function (response) {
            try {
                let data = response.data;
                const DeptRes = data.result;
                const RetRes = data.ReturnRes;
                //$('#modelTicketConfirmed').modal('show');
                if (DeptRes != '')
                {
                    bootbox.alert({
                        message: DeptRes + " AND " + RetRes,
                        callback: function () {
                            var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton"
                            $window.location.href = URL;
                            console.log('This was logged in the callback!');
                        }
                    });
                }
                else
                {
                    bootbox.alert({
                        message: RetRes,
                        callback: function () {
                            //var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton"
                            //$window.location.href = URL;
                            console.log('This was logged in the callback!');
                        }
                    });
                }
                
                //let data = JSON.parse(response.data);
                //if (data.BookTicketResponses.BookTicketResponse.length > 0) {
                //    //$('#modelTicketConfirmed').modal('show');
                //    bootbox.alert({
                //        message: "Your booking is confirmed.",
                //        callback: function () {
                //            var URL = "/Merchant/MerchantFlightDetails/BookedFlightInformaiton"
                //            $window.location.href = URL;
                //            console.log('This was logged in the callback!');
                //        }
                //    });
                //}
                //else {
                //    bootbox.alert({
                //        message: "Please check all the information and submit again.",
                //        callback: function () {
                //            //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                //            //$window.location.href = URL;
                //            //console.log('This was logged in the callback!');
                //        }
                //    });
                //}
            } catch (e) {
                bootbox.alert({
                    message: "Please check all the information and submit again.",
                    callback: function () {
                        //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
                        //$window.location.href = URL;
                        //console.log('This was logged in the callback!');
                    }
                });
            }


        });

    };

    //$scope.holdingFlightRequest = function (isFormInvalid) {
    //    if (isFormInvalid) {
    //        return false;
    //    }
    //    $scope.bookingRequestObj.RequestXml.BookTicketRequest.TrackNo = $scope.trackNumber;
    //    $scope.bookingRequestObj.RequestXml.BookTicketRequest.MobileNo = $scope.mobileNumber;
    //    $scope.bookingRequestObj.RequestXml.BookTicketRequest.AltMobileNo = $scope.altMobileNo;
    //    $scope.bookingRequestObj.RequestXml.BookTicketRequest.Email = $scope.emailAddress;
    //    $scope.bookingRequestObj.RequestXml.BookTicketRequest.Passengers.Passenger = $scope.passengers;
    //    $scope.bookingRequestObj.RequestXml.BookTicketRequest.Segments.Segment = $scope.segments;
    //    $scope.bookingRequestObj.RequestXml.BookTicketRequest.AdditionalServices.AdditionalService = $scope.additionaServices.filter(function (x) { return x.IsSelected; });
    //    $scope.bookingRequestObj.RequestXml.BookTicketRequest.TotalAmount = $scope.TotalAmount;
    //    if ($scope.flightDetails[0].HoldAllowed == 'Y') {
    //        $scope.bookingRequestObj.RequestXml.BookTicketRequest.HoldAllowed = 'Y';
    //        $scope.bookingRequestObj.RequestXml.BookTicketRequest.HoldCharge = $scope.flightDetails[0].HoldCharges;
    //    }
    //    const req = { req: JSON.stringify($scope.bookingRequestObj), userMarkup: $scope.userMarkup, FlightAmt: $scope.TotalAmount, TripMode: 'R', deptSegment: JSON.stringify($scope.deptureFlight), returnSegment: JSON.stringify($scope.returnFlight) };
    //    const service = FlightServices.getFlightHoldingServices(req);
    //    service.then(function (response) {
    //        try {
    //            let data = JSON.parse(response.data);
    //            if (data.BookTicketResponses.BookTicketResponse.length > 0) {
    //                //$('#modelTicketConfirmed').modal('show');
    //                bootbox.alert({
    //                    message: "Holding confirmed.",
    //                    callback: function () {
    //                        var URL = "/Merchant/MerchantFlightDetails";
    //                        $window.location.href = URL;
    //                        console.log('This was logged in the callback!');
    //                    }
    //                });
    //            }
    //            else {
    //                bootbox.alert({
    //                    message: "Please check all the information and submit again.",
    //                    callback: function () {
    //                        //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
    //                        //$window.location.href = URL;
    //                        //console.log('This was logged in the callback!');
    //                    }
    //                });
    //            }
    //        } catch (e) {
    //            bootbox.alert({
    //                message: "Please check all the information and submit again.",
    //                callback: function () {
    //                    //var URL = "/Merchant/MerchantFlightDetails/FlightBookingDetails";
    //                    //$window.location.href = URL;
    //                    //console.log('This was logged in the callback!');
    //                }
    //            });
    //        }
    //    });
    //};
    $scope.PrintFlightInvoice = function (ref_no, Pnr) {
        debugger;
    };

    $scope.totalAmountCalculation = function (amount) {
        if (amount) {
            
            return parseFloat(amount) + ($scope.additionalAddedAmount == 0 ? 0 : ($scope.additionalAddedAmount * 2)) + parseFloat($scope.userMarkup);
        }
        return 0;
    }
}]);

