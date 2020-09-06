app.controller('SingleFlightSearchApiCall', ['FlightServices', '$scope', '$http', '$window', '$filter', 'orderByFilter', function (FlightServices, $scope, $http, $window, $filter, orderBy) {


    $scope.additionalAddedAmount = parseFloat(document.getElementById('AIRADDITIONALAMOUNT').value);

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
        let returnValue = false;
       
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
            //debugger;
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
            
            // $scope.airlinesList.push({ name: 'For all Airlines', code: '', selected: false });

            const FlightSearchDetails = FlightResponse.GetFlightAvailibilityResponse.FlightDetails;
            $scope.FlightFareDetails = FlightResponse.GetFlightAvailibilityResponse.FareDetails;
            let objFlightSearchResult = $filter('groupBy')(FlightSearchDetails, 'TrackNo');
            $scope.flightsearchResult = Object.keys(objFlightSearchResult).map(function (key) {
                return objFlightSearchResult[key];
            });
            debugger;
            $scope.flightsearchResult = orderBy($scope.flightsearchResult, '[0].TotalAmount', false);

            const maxPeak = $scope.FlightFareDetails.reduce((p, c) => Math.round(p.NetAmount) > Math.round(c.NetAmount) ? p : c);
            const minPeak = $scope.FlightFareDetails.reduce((p, c) => Math.round(p.NetAmount) < Math.round(c.NetAmount) ? p : c);

            $scope.minAmount = Math.round(minPeak.NetAmount) + $scope.additionalAddedAmount;
            $scope.maxAmount = Math.round(maxPeak.NetAmount) + $scope.additionalAddedAmount;

            $scope.lower_price_bound = $scope.minAmount;
            $scope.upper_price_bound = $scope.maxAmount;
            $scope.min = $scope.minAmount;
            $scope.max = $scope.maxAmount;

            const myObj = { Time: new Date(), Token: FlightSearchDetails };
            localStorage.setItem('searchResult', null);
            localStorage.setItem('SearchTraceDetails', JSON.stringify(myObj));
            //localStorage.setItem('searchResult', JSON.stringify(FlightSearchDetails));
           

        });
    }

    $scope.$watch('airlinesList|filter:{selected:true}', function (nv) {
        $scope.filterData.airlines = nv.map(function (item) {
            return item.code;
        });
    }, true);



   


  
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


    $scope.timeConvert = function (n) {
        var minutes = n % 60
        var hours = (n - minutes) / 60
        return hours + " hr " + minutes + " m";
    }
    $scope.parseNumber = function (value) {
        return parseInt(value);
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
        return timeConvert(totalMinute);
    }
}]);