app.controller('FlightVerificationController', ['FlightServices', '$scope', '$http', '$window', function (FlightServices, $scope, $http, $window) {

    $scope.adult = 0;
    $scope.child = 0;
    $scope.infant = 0;
    $scope.passengers = [];
    $scope.trackNumber = '';
    let pessengerObj = {
        "PaxSeqNo": 0,
        "Title": "",
        "FirstName": "",
        "LastName": "",
        "PassengerType": "",
        "DateOfBirth": "",
        "PassportNo": "",
        "PassportExpDate": "",
        "PassportIssuingCountry": "IND",
        "NationalityCountry": "IND",
        "label":""
    };
    // reqObj.fl
    $scope.reqObj = {
        "RequestXml": {
            "Authenticate": {
                "InterfaceCode": "1",
                "InterfaceAuthKey": "AirticketOnlineWebSite",
                "AgentCode": "MLD0000001",
                "Password": "TEST1_"
            },
            "BookTicketRequest": {
                "TrackNo": $scope.trackNumber,
                "MobileNo": "9099776464",
                "AltMobileNo": "9898989898",
                "Email": "abc@gmail.com",
                "Address": "Test",
                "ClientRequestID": "",
                "Passengers": {
                    "Passenger": $scope.passengers
                    
                },
                "Segments": {
                    "Segment": [
                      {
                          "TrackNo": "0$30882|16|1SCO",
                          "SegmentSeqNo": "1",
                          "AirlineCode": "SG",
                          "FlightNo": "921",
                          "FromAirportCode": "AMD",
                          "ToAirportCode": "BLR",
                          "DepDate": "15/05/2019",
                          "DepTime": "05:50",
                          "ArrDate": "15/05/2019",
                          "ArrTime": "08:10",
                          "FlightClass": "CLS",
                          "MainClass": "Y"
                      },
                      {
                          "TrackNo": "0$30882|16|1SCO",
                          "SegmentSeqNo": "2",
                          "AirlineCode": "SG",
                          "FlightNo": "657",
                          "FromAirportCode": "BLR",
                          "ToAirportCode": "CCU",
                          "DepDate": "15/05/2019",
                          "DepTime": "10:40",
                          "ArrDate": "15/05/2019",
                          "ArrTime": "13:05",
                          "FlightClass": "CLS",
                          "MainClass": "Y"
                      }
                    ]
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
                "MerchantCode": "",
                "MerchantKey": "",
                "SaltKey": "    ",
                "IsTicketing": "Yes"
            }
        }
    };

    
    $scope.loadFlightDetails = function (trackNo, tripMode) {
        const data = { TrackNo: trackNo, TripMode: tripMode };
        const service = FlightServices.getFlightVerificationDetails(data);
        service.then(function (response) {
            const data = response.data;
            const verifyFlightDetailResponse = JSON.parse(JSON.parse(data)).VerifyFlightDetailResponse;
            const flightDetails = verifyFlightDetailResponse.FlightDetails;
            const fareDetails = verifyFlightDetailResponse.FareDetails;

            $scope.flightDetails = flightDetails;
            $scope.fareDetails = fareDetails;
            $scope.detailsLoadingError = verifyFlightDetailResponse.Error;
            $scope.trackNumber = $scope.flightDetails[0].TrackNo;

            console.log($scope.flightDetails)
            console.log($scope.fareDetails)
        });
    }

    $scope.populatePesengerList = function (adult, child, infant) {
        $scope.passengers = [];
        $scope.adult = adult ? parseInt(adult) : 0;
        debugger;
        if ($scope.adult > 0) {
            for (let i=1; i <= $scope.adult; i++)
            {
                let objCopy = Object.assign({}, pessengerObj);
                objCopy.PaxSeqNo = i;
                objCopy.PassengerType = "A";
                objCopy.label = `Adult ${i}`;
                $scope.passengers.push(objCopy)
            }
        }
        $scope.child = child ? parseInt(child) : 0;
        if ($scope.child > 0) {
            for (let i=1; i <= $scope.child; i++) {
                let objCopy = Object.assign({}, pessengerObj);
                objCopy.PaxSeqNo = $scope.passengers.length + i;
                objCopy.PassengerType = "C";
                objCopy.label = `Child ${i}`;
                $scope.passengers.push(objCopy)
            }
        }
        $scope.infant = infant ? parseInt(infant) : 0;
        if ($scope.infant > 0) {
            for (let i=1; i <= $scope.infant; i++) {
                let objCopy = Object.assign({}, pessengerObj);
                objCopy.PaxSeqNo = $scope.passengers.length + i;
                objCopy.PassengerType = "I";
                objCopy.label = `Infant ${i}`;
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

    $scope.calculateDurationTime = function(n)
    {
        var minutes = n % 60
        var hours = (n - minutes) / 60
        return hours + " hr " + minutes + " m";
    }

}]);

