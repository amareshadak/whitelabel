app.controller('ReturnFlightSearchController', ['FlightServices', '$scope', '$http', '$window', '$filter', function (FlightServices, $scope, $http, $window, $filter) {
    $scope.lower_price_bound = 0;
    $scope.upper_price_bound = 1000;
    $scope.min = 0;
    $scope.max = 1000;
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

    $scope.filterRetrunData = {
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
    $scope.selectDeptTrackNo = '';
    $scope.selectReturnTrackNo = '';



    $scope.filterDeptFlightData = function (item) {
        
        let returnValue = false;

        let amount = Math.round(item[0].TotalAmount);
        returnValue = Math.round(amount) >= $scope.lower_price_bound && Math.round(amount) <= $scope.upper_price_bound;

        if ($scope.filterData.timeSlots.EarlyMorning
            || $scope.filterData.timeSlots.Morning
            || $scope.filterData.timeSlots.MidDay
            || $scope.filterData.timeSlots.Evening
            || $scope.filterData.timeSlots.Night) {

            let checkValue = [];
            let time = parseInt(item[0].DepTime.replace(':', ''));

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


    $scope.filterReturnFlightData = function (item) {
        let returnValue = false;
      
        let amount = Math.round(item[0].TotalAmount);
        returnValue = Math.round(amount) >= $scope.lower_price_bound && Math.round(amount) <= $scope.upper_price_bound;

        if ($scope.filterRetrunData.timeSlots.EarlyMorning
            || $scope.filterRetrunData.timeSlots.Morning
            || $scope.filterRetrunData.timeSlots.MidDay
            || $scope.filterRetrunData.timeSlots.Evening
            || $scope.filterRetrunData.timeSlots.Night) {

            let checkValue = [];
            let time = parseInt(item[0].DepTime.replace(':', ''));

            if ($scope.filterRetrunData.timeSlots.EarlyMorning) { // 0 to 8
                checkValue.push([0, 800]);
            }

            if ($scope.filterRetrunData.timeSlots.Morning) { // 8 to 12
                checkValue.push([800, 1200]);
            }

            if ($scope.filterRetrunData.timeSlots.MidDay) { // 12 to 16
                checkValue.push([1200, 1600]);
            }

            if ($scope.filterRetrunData.timeSlots.Evening) { // 16 to 20
                checkValue.push([1600, 2000]);
            }

            if ($scope.filterRetrunData.timeSlots.Night) { // 20 to 24
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

    $scope.selectedDeptFlight = [];
    $scope.selectedReturnFlight = [];

    $scope.LoadFlightSearch = function (Tripmode, FromCityCode, TOAirportCode, FromDate, ToDate, TravelType, Adult, Child, Infant) {
        //const data = { TrackNo: trackNo, TripMode: tripMode };
        // debugger;
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

        const service = FlightServices.getFlightReturnSearchDetails(data);
        service.then(function (response) {
            const data = response.data;
            const FlightResponse = JSON.parse(data);
            const info = FlightResponse.GetFlightAvailibilityResponse;

            $scope.airlinesList = FlightResponse.GetFlightAvailibilityResponse.AirlineList.map(item => {
                const container = {};
                container.name = item.AirlineName;
                container.code = item.AirlineCode;
                container.selected = false;
                return container;
            });
            
            
            const FlightSearchDetails = FlightResponse.GetFlightAvailibilityResponse.FlightDetails;
            $scope.FlightFareDetails = FlightResponse.GetFlightAvailibilityResponse.FareDetails;

            const deptureFlight = FlightSearchDetails.filter(x => x.SrNo.charAt(x.SrNo.length - 1) === 'O');
            const returnFlight = FlightSearchDetails.filter(x => x.SrNo.charAt(x.SrNo.length - 1) === 'R');
            
            let objdeptureFlightSearchResult = $filter('groupBy')(deptureFlight, 'TrackNo');
            $scope.deptureFlight = Object.keys(objdeptureFlightSearchResult).map(function (key) {
                return objdeptureFlightSearchResult[key];
            });
            let objReturnFlightSearchResult = $filter('groupBy')(returnFlight, 'TrackNo');
            $scope.returnFlight = Object.keys(objReturnFlightSearchResult).map(function (key) {
                return objReturnFlightSearchResult[key];
            });
            $scope.selectedDeptFlight = $scope.deptureFlight[0];
            $scope.selectedReturnFlight = $scope.returnFlight[0];
            $scope.selectDeptTrackNo = $scope.selectedDeptFlight[0].TrackNo;
            $scope.selectReturnTrackNo = $scope.selectedReturnFlight[0].TrackNo;


            const maxPeak = $scope.FlightFareDetails.reduce((p, c) => Math.round(p.NetAmount) > Math.round(c.NetAmount) ? p : c);
            const minPeak = $scope.FlightFareDetails.reduce((p, c) => Math.round(p.NetAmount) < Math.round(c.NetAmount) ? p : c);

            $scope.minAmount = Math.round(minPeak.NetAmount);
            $scope.maxAmount = Math.round(maxPeak.NetAmount);

            $scope.lower_price_bound = $scope.minAmount;
            $scope.upper_price_bound = $scope.maxAmount;
            $scope.min = $scope.minAmount;
            $scope.max = $scope.maxAmount;

            //const myObj = { Time: new Date(), Token: FlightSearchDetails };
            //localStorage.setItem('searchResult', null);
            //localStorage.setItem('SearchTraceDetails', JSON.stringify(myObj));
            //localStorage.setItem('searchResult', JSON.stringify(FlightSearchDetails));


        });
    }

    $scope.$watch('airlinesList|filter:{selected:true}', function (nv) {
        $scope.filterData.airlines = nv.map(function (item) {
            return item.code;
        });
    }, true);







    $scope.loadDeptFareDetails = function (srNo, adult, child, infant) {
        let adlt = adult;
        let data = $scope.FlightFareDetails.filter(x => x.SrNo == srNo);
        let adultval = (adult = 0 ? 0 : adult);
        let childval = (child = 0 ? 0 : child);
        let infantval = (infant = 0 ? 0 : infant);
        let FlightBaseFare = (parseFloat(data[0].AdultBaseFare)) + (parseFloat(data[0].ChildBaseFare)) + (parseFloat(data[0].InfantBaseFare));
        $scope.FlightDeptTotalBasefare = FlightBaseFare;       
        let FlightTaxFare = (parseFloat(data[0].AdultTax)) + (parseFloat(data[0].ChildTax)) + (parseFloat(data[0].InfantTax));
        $scope.FlightDeptTotalTaxfare = FlightTaxFare;
        let FlightCuteFare = (parseFloat(data[0].AdultCuteFee)) + (parseFloat(data[0].ChildCuteFee)) + (parseFloat(data[0].InfantCuteFee));
        $scope.FlightDeptTotalCutefare = FlightCuteFare;
        $scope.FlightDeptTotalFare = parseFloat(FlightBaseFare) + parseFloat(FlightTaxFare) + parseFloat(FlightCuteFare);
        let baggageval = data[0].Baggage;
        $scope.BaggageDeptAllowance = baggageval;
    }

    $scope.loadRetrnFareDetails = function (srNo, adult, child, infant) {
        let adlt = adult;
        let data = $scope.FlightFareDetails.filter(x => x.SrNo == srNo);
        let adultval = (adult = 0 ? 0 : adult);
        let childval = (child = 0 ? 0 : child);
        let infantval = (infant = 0 ? 0 : infant);
        let FlightBaseFare = (parseFloat(data[0].AdultBaseFare)) + (parseFloat(data[0].ChildBaseFare)) + (parseFloat(data[0].InfantBaseFare));
        $scope.FlightReturnTotalBasefare = FlightBaseFare;
       
        let FlightTaxFare = (parseFloat(data[0].AdultTax)) + (parseFloat(data[0].ChildTax)) + (parseFloat(data[0].InfantTax));
        $scope.FlightReturnTotalTaxfare = FlightTaxFare;

        let FlightCuteFare = (parseFloat(data[0].AdultCuteFee)) + (parseFloat(data[0].ChildCuteFee)) + (parseFloat(data[0].InfantCuteFee));
        $scope.FlightReturnTotalCutefare = FlightCuteFare;

        $scope.FlightReturnTotalFare = parseFloat(FlightBaseFare) + parseFloat(FlightTaxFare) + parseFloat(FlightCuteFare);
        let baggageval = data[0].Baggage;
        $scope.BaggageReturnAllowance = baggageval;
    }

    $scope.setSelectedDeptFlight = function (item) {
        $scope.selectedDeptFlight = angular.copy(item);
        $scope.selectDeptTrackNo = $scope.selectedDeptFlight[0].TrackNo;
    }
    $scope.setSelectedReturnFlight = function (item) {
        $scope.selectedReturnFlight = angular.copy(item);
        $scope.selectReturnTrackNo = $scope.selectedReturnFlight[0].TrackNo;
    }

    

    //$scope.selectDestimationSource = function (TrackNo, FromAirportCode, DepTime, Via, TotalDur, Stops, TOAirportCode, ArivTime, DepDate, TotalAmount, SrNo, FlightNo, AirlineCode) {
    $scope.selectDestimationSource = function (ItemVal) {
        // debugger;
        //var FlightSelect = $scope.deptureFlight[$index];
        $scope.selectval = true;
        $scope.DeptAirLineCode = ItemVal[0].AirlineCode;
        $scope.OriginResultIndex = SrNo;
        $scope.FlightName = FromAirportCode;
        //$scope.FlightCode = FlightSelect.FlightNo;
        $scope.FlightNo = FlightNo;
        $scope.ViaFromStopage = Via;
        $scope.Duration = TotalDur;
        $scope.FromStops = Stops;
        $scope.OriginAirportCode = FromAirportCode;
        $scope.Departtime = DepTime;
        $scope.DepartDate = DepDate;
        $scope.Reachedtime = ArivTime;
        //$scope.ReachedDate = FlightSelect.DepDate;
        $scope.DestinationAirportcode = TOAirportCode;
        $scope.DeparturePublishedFare = TotalAmount;
        $scope.SourceISLCC = TrackNo;
        $scope.FlightPrice = TotalAmount;
        //$scope.FlightTaxPrice = FlightSelect.TaxAmount;
        $scope.FlightTrackNo = TrackNo;
        //console.log(FlightPrice);
    }
    $scope.selectReturnFlightSource = function (TrackNo, FromAirportCode, DepTime, Via, TotalDur, Stops, TOAirportCode, ArivTime, DepDate, TotalAmount, SrNo, FlightNo, AirlineCode) {
        // debugger;
        //var FlightSelect = $scope.flightsearchResult.FlightDetails[$index];
        //var FlightSelect = $scope.returnFlight[$index];
        $scope.selectval = true;
        $scope.ReturnAirLineCode = AirlineCode;
        $scope.DesOriginResultIndex = SrNo;
        $scope.DesFlightName = FromAirportCode;
        //$scope.DesFlightCode = FlightSelect.FlightNo;
        $scope.DesFlightNo = FlightNo;
        $scope.retvia = Via;
        $scope.DesDuration = TotalDur;
        $scope.DesFromStops = Stops;
        $scope.DesOriginAirportCode = FromAirportCode;
        $scope.DesDeparttime = DepTime;
        $scope.DesDepartDate = DepDate;
        $scope.DesReachedtime = ArivTime;
        //$scope.DesReachedDate = FlightSelect.DepDate;
        $scope.DesDestinationAirportcode = TOAirportCode;
        $scope.DesDeparturePublishedFare = TotalAmount;
        $scope.DesSourceISLCC = TrackNo;
        $scope.DesFlightPrice = TotalAmount;
        //$scope.DesFlightTaxPrice = FlightSelect.TaxAmount;
        $scope.DesFlightTrackNo = TrackNo;

        //console.log(DesFlightPrice);
    }

    $scope.RoundtripgetFlightDetails = function (Adult, Children, Infant, TrackNo, TripMode) {
        // debugger;
        //var Tracevalue = JSON.parse(window.localStorage.getItem("SearchTraceDetails"));
        //var timevalue = new Date(Tracevalue.Time);
        //var TraceId = Tracevalue.Token;
        //const currDate = new Date();
        //const oldDate = timevalue;
        window.location.href = '/Merchant/MerchantFlightDetails/FlightBookingDetails?TrackNo=' + TrackNo + '&PsgnAdult=' + Adult + '&PsgnChildren=' + Children + '&PsgnInfant=' + Infant + '&TripMode=' + TripMode;

    };

    $scope.AddTotalAmount = function (DerpAmt, RetAmt) {   
        if (DerpAmt && RetAmt) {
            let TotalAmt = 0;
            if (DerpAmt[0].TotalAmount && RetAmt[0].TotalAmount != undefined) {
                TotalAmt = parseFloat(DerpAmt[0].TotalAmount) + parseFloat(RetAmt[0].TotalAmount);
            }
            else {
                TotalAmt = 0;
            }
            return Math.round(TotalAmt);
        }
    }


    $scope.timeConvert = function (n) {
        var minutes = n % 60
        var hours = (n - minutes) / 60
        return hours + " hr " + minutes + " m";
    }

    $scope.parseNumber = function (value) {
        return parseInt(value);
    }

    $scope.parseFloat = function (value) {
        return parseFloat(value);
    }

    $scope.getFlightDetails = function (Adult, Children, Infant, TrackNo, TripMode) {
        var Tracevalue = JSON.parse(window.localStorage.getItem("SearchTraceDetails"));
        var timevalue = new Date(Tracevalue.Time);
        var TraceId = Tracevalue.Token;
        const currDate = new Date();
        const oldDate = timevalue;

        var list = (currDate - oldDate) / 60000;
        if (list <= 15) {
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

    function timeConvert(n) {
        var num = n;
        var hours = (num / 60);
        var rhours = Math.floor(hours);
        var minutes = (hours - rhours) * 60;
        var rminutes = Math.round(minutes);
        return rhours + "h " + rminutes + "m";
    }

    $scope.calculateDuration = function (item) {
        if (item) {
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
            return timeConvert(totalMinute);
        }
        
    }
}]);