app.controller('SingleFlightSearchApiCall', ['FlightServices', '$scope', '$http', '$window', '$filter', 'orderByFilter', function (FlightServices, $scope, $http, $window, $filter, orderBy) {


    $scope.additionalAddedAmount = 0;//parseFloat(document.getElementById('AIRADDITIONALAMOUNT').value);

    $scope.lower_price_bound = 0;
    $scope.upper_price_bound = 1000;
    $scope.min = 0;
    $scope.max = 1000;

    $scope.dynamicPopover = {
        content: '',
        templateUrl: 'myPopoverTemplate.html',
        title: ''
    };

    $scope.getFloatNumber = function (n) {
        if (n) {
            return parseFloat(n);
        }
        return 0;
    }


    $scope.filterData = {
        stops: null,
        timeSlots: {
            EarlyMorning: false,
            Morning: false,
            MidDay: false,
            Evening: false,
            Night: false
        },
        refundable: null,
        airlines: []
    };

    $scope.airlinesList = [];



    $scope.filterFlightData = function (item) {
        let returnValue = true;
       
        let amount = Math.round(item[0].TotalAmount);
        returnValue = Math.round(amount) >= ($scope.lower_price_bound - $scope.additionalAddedAmount) && Math.round(amount) <= ($scope.upper_price_bound - $scope.additionalAddedAmount);

        if (($scope.filterData.timeSlots.EarlyMorning
            || $scope.filterData.timeSlots.Morning
            || $scope.filterData.timeSlots.MidDay
            || $scope.filterData.timeSlots.Evening
            || $scope.filterData.timeSlots.Night) && returnValue) {

            let checkValue = [];
            let time =parseInt(item[0].DepTime.replace(':',''));

            if ($scope.filterData.timeSlots.EarlyMorning) { // 0 to 8
                checkValue.push([0, 800]);
            }

            if ($scope.filterData.timeSlots.Morning) { // 8 to 12
                checkValue.push([800, 1200]);
            }

            if ($scope.filterData.timeSlots.MidDay) { // 12 to 16
                checkValue.push([1200, 1600]);
            }

            if ($scope.filterData.timeSlots.Evening) { // 16 to 20
                checkValue.push([1600, 2000]);
            }

            if ($scope.filterData.timeSlots.Night) { // 20 to 24
                checkValue.push([2000, 2400]);
            }

            let check = false;
            for (var i = 0; i < checkValue.length; i++) {
                if (checkValue[i][0] <= time && checkValue[i][1] >= time) {
                    check = true;
                    break;
                }
            }
            returnValue = check;
        }
       
        if ($scope.filterData.stops != null && returnValue) {
            returnValue = item[0].Stops == $scope.filterData.stops;
        }

        if ($scope.filterData.refundable != null && returnValue) {
            returnValue = item[0].FareType == ($scope.filterData.refundable == 'true' ? 'R' : 'N');
        }
      
        if ($scope.filterData.airlines.length > 0 && returnValue) {
            returnValue = $scope.filterData.airlines.includes(item[0].AirlineCode);
        }
       
       

        return returnValue;
       
    };

    $scope.resetFilterAirlinesList = function () {

        $scope.airlinesList = $scope.airlinesList.map(item => {
            const container = {};
            container.name = item.name;
            container.code = item.code;
            container.selected = false;
            return container;
        });
    }

    $scope.LoadFlightSearch = function (Tripmode, FromCityCode, TOAirportCode, FromDate, ToDate, TravelType, Adult, Child, Infant) {
        //const data = { TrackNo: trackNo, TripMode: tripMode };
        
        $scope.formdisplay = true;
        var Tripmode = Tripmode;
        var FromAirportsName = FromCityCode;
        var FromCityCode = FromCityCode;
        var TOAirportName = TOAirportCode;
        var TOAirportCode = TOAirportCode;
        var FromDate_Value = FromDate;
        var ToDate = ToDate;
        var TravelType = TravelType;
        var Adult = Adult;
        var Child = Child;
        var Infant = Infant;

        $scope.FromCityCodeVal = FromCityCode;
        $scope.ToDistination = TOAirportCode;
        $scope.TripMode = Tripmode;
        $scope.AdultCount = Adult;
        $scope.ChildCount = Child;
        $scope.InfantCount = Infant;
        
        const data = {
            Tripmode: Tripmode,
            FromAirportsName: FromAirportsName,
            FromCityCode: FromCityCode,
            TOAirportName: TOAirportName,
            TOAirportCode: TOAirportCode,
            FromDate: FromDate_Value,
            ToDate: ToDate,
            TravelType: TravelType,
            Adult: Adult,
            Child: Child,
            Infant: Infant
        };

        localStorage.setItem('SEARCH_FLIGHT_DATA', JSON.stringify(data));

        const service = FlightServices.getFlightSingleSearchDetails(data);
        service.then(function (response) {

            const data = response.data;
            const FlightResponse = JSON.parse(data);

            
            $scope.airlinesList = FlightResponse.GetFlightAvailibilityResponse.AirlineList.map(item => {
                const container = {};
                container.name = item.AirlineName;
                container.code = item.AirlineCode;
                container.selected = false;
                return container;
            });
            
            //=========================== Store All Flight Information =============================//
            $scope.flightDetails = FlightResponse.GetFlightAvailibilityResponse.FlightDetails;
            $scope.fareDetails = FlightResponse.GetFlightAvailibilityResponse.FareDetails;
            $scope.airlineList = FlightResponse.GetFlightAvailibilityResponse.AirlineList;
            $scope.airportList = FlightResponse.GetFlightAvailibilityResponse.AirportList;

            //================= Group by track number =============================//
            let objFlightSearchResult = $filter('groupBy')($scope.flightDetails, 'TrackNo');

            //================= Store a copy for feature uses =======================//
            $scope.flightsearchResultTrackNo = Object.keys(objFlightSearchResult).map(function (key) {
                return objFlightSearchResult[key];
            });

            //================= Conver Object to Array ============================//
            $scope.flightsearchResult = Object.keys(objFlightSearchResult).map(function (key) {
                return objFlightSearchResult[key];
            });

            $scope.flightNumber = [];
            //================== Rmove Same Flight With Diffrent Amount =============//
            $scope.flightsearchResult = $scope.flightsearchResult.filter(function (x) {
                let retVal = true;
                let createUniqueFlight = "";
                for (let index = 0; index < x.length; index++) {
                    const element = x[index];
                    createUniqueFlight = createUniqueFlight + element.FlightNo;
                }
                let check = $scope.flightNumber.indexOf(createUniqueFlight);
                if (check == -1) {
                    $scope.flightNumber.push(createUniqueFlight);
                } else {
                    return false;
                }
                return retVal;
            });

            
            $scope.flightFareList = [];
         
            //================== Preaper Main Flight List and Fare List ========================//
            for (let i = 0; i < $scope.flightsearchResult.length; i++) {
            
                const element = $scope.flightsearchResult[i];
                let result = $scope.getSameFlightList(element);
                let checkdata = $scope.flightDetails.filter(function (x) {
                    return x.TrackNo == result[0].TrackNo;
                });
                $scope.flightsearchResult[i] = checkdata;


                let fare = $scope.fareDetails.filter(function (item) {
                    return item.SrNo == checkdata[0].SrNo;
                });
                
                $scope.flightFareList.push(fare[0])
            }

            //==================== Order By Amount =============================//
            $scope.flightsearchResult.sort(function (a, b) {
                var valueA, valueB;
                valueA = Math.round(a[0].TotalAmount); 
                valueB = Math.round(b[0].TotalAmount);
                if (valueA < valueB) {
                    return -1;
                } else if (valueA > valueB) {
                    return 1;
                }
                return 0;
            });


            const maxPeak = $scope.fareDetails.reduce((p, c) => Math.round(p.TotalAmount) > Math.round(c.TotalAmount) ? p : c);
            const minPeak = $scope.fareDetails.reduce((p, c) => Math.round(p.TotalAmount) < Math.round(c.TotalAmount) ? p : c);

            $scope.minAmount = Math.round(minPeak.TotalAmount) + $scope.additionalAddedAmount;
            $scope.maxAmount = Math.round(maxPeak.TotalAmount) + $scope.additionalAddedAmount;

            $scope.lower_price_bound = $scope.minAmount;
            $scope.upper_price_bound = $scope.maxAmount;

            $scope.min = $scope.minAmount;
            $scope.max = $scope.maxAmount;

            const myObj = { Time: new Date(), Token: '' };
            localStorage.setItem('searchResult', null);
            localStorage.setItem('SearchTraceDetails', JSON.stringify(myObj));

           

        });
    }

    $scope.getSameFlightList = function (flight) {
        if (flight) {
            let createUniqueFlight = "";
            for (let index = 0; index < flight.length; index++) {
                const element = flight[index];
                createUniqueFlight = createUniqueFlight + element.FlightNo;
            }
            let checkdataTrackNo = $scope.flightsearchResultTrackNo.filter(function (x) {
                let createUniqueFlightItem = "";
                for (let i = 0; i < x.length; i++) {
                    const element = x[i];
                    createUniqueFlightItem = createUniqueFlightItem + element.FlightNo;
                }
                return createUniqueFlight == createUniqueFlightItem;
            });
            let checkdata = checkdataTrackNo.map(function (x) {
                return x[0];
            });
            checkdata.sort(function (a, b) {
                let valueA, valueB;
                valueA = Math.round(a.TotalAmount);
                valueB = Math.round(b.TotalAmount);
                if (valueA < valueB) {
                    return -1;
                } else if (valueA > valueB) {
                    return 1;
                }
                return 0;
            });
            return checkdata;
        }
       
    };

    $scope.setSameFareFlight = function (trackNo, flightNo, index) {
        if (trackNo) {
            let checkdata = $scope.flightDetails.filter(function (x) {
                return x.TrackNo == trackNo;
            });
            $scope.flightsearchResult[index] = checkdata;
        }
    };



    $scope.getAirlineName = function (code) {
        let data = $scope.airlineList.filter(function (x) {
            return x.AirlineCode == code;
        });

        return data[0].AirlineName;
    };

    $scope.getAirportName = function (code) {
        let data = $scope.airportList.filter(function (x) {
            return x.AirportCode == code;
        });

        return data[0].AirportName;
    };

    $scope.getFormatDate = function (date) {
        let splitDate = date.split("/");
        return moment(`${splitDate[1]}/${splitDate[0]}/${splitDate[2]}`).format(
            "ddd D MMM YY"
        );
    };


    $scope.$watch('airlinesList|filter:{selected:true}', function (nv) {
        $scope.filterData.airlines = nv.map(function (item) {
            return item.code;
        });
    }, true);



    $scope.getFlightTotalBasefare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.fareDetails.filter(function (item) {
                return item.SrNo == SrNo;
            });
            if (fare) {
                return (
                    (parseFloat(fare[0].AdultBaseFare)) +
                    (parseFloat(fare[0].ChildBaseFare)) +
                    (parseFloat(fare[0].InfantBaseFare))
                );
            }
            else {
                return 0;
            }
        }
        return 0;
    }

    $scope.getOtherFare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.fareDetails.filter(function (item) {
                return item.SrNo == SrNo;
            });
            if (fare) {
                return (
                    parseFloat(fare[0].AdultTax) +
                    parseFloat(fare[0].ChildTax) +
                    parseFloat(fare[0].InfantTax) +
                    parseFloat(fare[0].AdultFuelCharges) +
                    parseFloat(fare[0].ChildFuelCharges) +
                    parseFloat(fare[0].InfantFuelCharges) +
                    parseFloat(fare[0].AdultPassengerServiceFee) +
                    parseFloat(fare[0].ChildPassengerServiceFee) +
                    parseFloat(fare[0].InfantPassengerServiceFee) +
                    parseFloat(fare[0].AdultTransactionFee) +
                    parseFloat(fare[0].ChildTransactionFee) +
                    parseFloat(fare[0].InfantTransactionFee) +
                    parseFloat(fare[0].AdultServiceCharges) +
                    parseFloat(fare[0].ChildServiceCharges) +
                    parseFloat(fare[0].InfantServiceCharges) +
                    parseFloat(fare[0].AdultAirportTax) +
                    parseFloat(fare[0].ChildAirportTax) +
                    parseFloat(fare[0].InfantAirportTax) +
                    parseFloat(fare[0].AdultAirportDevelopmentFee) +
                    parseFloat(fare[0].AdultCuteFee) +
                    parseFloat(fare[0].AdultConvenienceFee) +
                    parseFloat(fare[0].AdultSkyCafeMeals) +
                    parseFloat(fare[0].ChildAirportDevelopmentFee) +
                    parseFloat(fare[0].ChildCuteFee) +
                    parseFloat(fare[0].ChildConvenienceFee) +
                    parseFloat(fare[0].ChildSkyCafeMeals) +
                    parseFloat(fare[0].InfantAirportDevelopmentFee) +
                    parseFloat(fare[0].InfantCuteFee) +
                    parseFloat(fare[0].InfantConvenienceFee) +
                    parseFloat(fare[0].InfantSkyCafeMeals)
                );
            }
            else {
                return 0;
            }
        }
        return 0;
    };


    $scope.getBaggagesInfo = function (SrNo) {
        if (SrNo) {
            let fare = $scope.fareDetails.filter(function (item) {
                return item.SrNo == SrNo;
            });
            if (fare) {
                return fare[0].Baggage;
            }
            else {
                return 0;
            }
        }
        return 0;
    };

  
    $scope.loadFareDetails = function (srNo,adult,child,infant)
    {
        var adlt = adult;
        let data = $scope.FlightFareDetails.filter(x => x.SrNo == srNo);
        var adultval = (adult = 0 ? 0 : adult);
        var childval = (child = 0 ? 0 : child);
        var infantval = (infant = 0 ? 0 : infant);
        //var basefare = data[0].AdultBaseFare;
        //var FlightBaseFare = (parseFloat(data[0].AdultBaseFare) * adultval) + (parseFloat(data[0].ChildBaseFare) * childval) + (parseFloat(data[0].InfantBaseFare) * infantval);
        var FlightBaseFare = (parseFloat(data[0].AdultBaseFare)) + (parseFloat(data[0].ChildBaseFare)) + (parseFloat(data[0].InfantBaseFare));
        $scope.FlightTotalBasefare = FlightBaseFare;
        //var FlightTaxFare = (parseFloat(data[0].AdultTax) * adultval) + (parseFloat(data[0].ChildTax) * childval) + (parseFloat(data[0].InfantTax) * infantval);
        var FlightTaxFare = (parseFloat(data[0].AdultTax)) + (parseFloat(data[0].ChildTax) ) + (parseFloat(data[0].InfantTax) );
        $scope.FlightTotalTaxfare = FlightTaxFare;

        var FlightCuteFare = (parseFloat(data[0].AdultCuteFee)) + (parseFloat(data[0].ChildCuteFee)) + (parseFloat(data[0].InfantCuteFee));
        $scope.FlightTotalCutefare = FlightCuteFare;

        $scope.FlightTotalFare = parseFloat(FlightBaseFare) + parseFloat(FlightTaxFare) + parseFloat(FlightCuteFare);
        var baggageval = data[0].Baggage;
        $scope.BaggageAllowance = baggageval;
    }


   
    $scope.parseNumber = function (value) {
        return parseInt(value);
    }

    function millisToMinutesAndSeconds(millis) {
        var minutes = Math.floor(millis / 60000);
        var seconds = ((millis % 60000) / 1000).toFixed(0);
        return parseFloat(minutes); // + ":" + (seconds < 10 ? '0' : '') + seconds;
    }

    $scope.getFlightDetails = function (Adult, Children, Infant, TrackNo, TripMode) {
        debugger;
        var Tracevalue = JSON.parse(window.localStorage.getItem("SearchTraceDetails"));

        const diffInMilliseconds = Math.abs(new Date() - new Date(Tracevalue.Time));

        if (millisToMinutesAndSeconds(diffInMilliseconds) <= 15) {
            //window.location.href = '/Merchant/MerchantFlightBooking/FlightBooking?BookingValue=' + item + '&token=' + TraceId + '&Passenger=' + Passenger + '&TripMode=' + TripMode + '&IsLCC=' + IsLCC;;
            window.location.href = '/Merchant/MerchantFlightDetails/FlightBookingDetails?TrackNo=' + TrackNo + '&PsgnAdult=' + Adult + '&PsgnChildren=' + Children + '&PsgnInfant=' + Infant + '&TripMode=' + TripMode;
        }
        else {
            bootbox.alert({
                message: "Session is expired.Please search again",
                backdrop: true
            });
        }
    };

    function diff_minutes(dt2, dt1) {

        var diff = (dt2.getTime() - dt1.getTime()) / 1000;
        diff /= 60;
        return Math.abs(Math.round(diff));

    }

    $scope.timeConvert = function(n) {
        var num = n;
        var hours = (num / 60);
        var rhours = Math.floor(hours);
        var minutes = (hours - rhours) * 60;
        var rminutes = Math.round(minutes);
        return  rhours + "h " + rminutes + "m";
    }

    $scope.calculateDuration = function(item)
    {
        const firstDate = item[0].DepDate;
        const firstTime = item[0].DepTime;
        const lastDate = item[item.length - 1].ArrDate;
        const lastTime = item[item.length - 1].ArrTime;
        const firstDateArray = firstDate.split('/');
        const firstTimeArray = firstTime.split(':');
        const lastDateArray = lastDate.split('/');
        const lastTimeArray = lastTime.split(':');
        const firstDateTime = new Date(firstDateArray[2], firstDateArray[1], firstDateArray[0], firstTimeArray[0], firstTimeArray[1]);
        const lastDateTime = new Date(lastDateArray[2], lastDateArray[1], lastDateArray[0], lastTimeArray[0], lastTimeArray[1]);
        const totalMinute = diff_minutes(lastDateTime, firstDateTime);
        return $scope.timeConvert(totalMinute);
    }
}]);