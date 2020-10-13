
//var app = angular.module('AirportAutocompleteoduleApp', ['angucomplete-alt', 'angular.filter']); //add angucomplete-alt dependency in app
//app.controller('AirportAutocompleteController', function ($scope, $http, $window, $location, $filter) {
app.controller('AirportAutocompleteController', function ($scope, $http, $window, $location, $filter, $timeout) {
    $scope.numberToDisplay = 10;
    $scope.flightsearchResult = [];
    $scope.index = 0;
    $scope.filterhtml = false;
    $scope.selectedAirlines = new Array();
    $scope.airlines = new Array();

    
    $scope.startDatePopup = {
        opened: false
    };

    $scope.openStartDatePicker = function () {
        $scope.startDatePopup.opened = true;
    };

    $scope.endDatePopup = {
        opened: false
    };

    $scope.openEndDatePicker = function () {
        $scope.endDatePopup.opened = true;
    };
    $scope.$watch('startDatePopup.opened', function (oldValue, newValue) {
        if ($scope.startDatePopup.opened) {
            $timeout(function () {
                changeMonth();
            }, 1000);
        }
    });


    $scope.$watch('endDatePopup.opened', function (oldValue, newValue) {
        if ($scope.endDatePopup.opened) {
            $timeout(function () {
                changeMonth();
            }, 1000);
        }
    });



    $scope.AddMulticitySearchInfo = new Array();

    $scope.addNewRow = function () {     
        debugger;
        //CITYCODE
        //CITYNAME
        if ($scope.SelectedAirport.CITYCODE != null) {
            $scope.AddMulticitySearchInfo.push({
                Source: $scope.SelectedAirport.CITYCODE,
                Destination: $scope.ToAirportName.CITYCODE,
                DateofJourney: $scope.FromDate
            });
        }
        else {
            $scope.AddMulticitySearchInfo.push({
                Source: $scope.SelectedAirport.description.CITYCODE,
                Destination: $scope.ToAirportName.description.CITYCODE,
                DateofJourney: $scope.FromDate
            });
        }
        $scope.$broadcast('angucomplete-alt:clearInput')
        //$scope.SelectedAirport.CITYNAME = '';
        //$scope.ToAirportName.CITYNAME = '';
        ////$scope.Airport.ToAirportsName = '';
        //$('#txtAutocomplete').val('');
        //$('#txtTOAutocomplete').val('');
        //$scope.ToAirportName.CITYCODE = '';
        Source = '';
        Destination = '';
        DateofJourney = '';

        //console.log(AddMulticitySearchInfo);
    }

    $scope.Remove = function (index) {
        debugger;
        //Find the record using Index from Array.
        //var name = $scope.AddMulticitySearchInfo[index].Source;
        //if ($window.confirm("Do you want to delete: " + Source)) {
        //    //Remove the item from Array using Index.
            $scope.AddMulticitySearchInfo.splice(index, 1);
        //}
    }

    //New Add For Traveler List
    $scope.adultTypeTrv = '1';
    $scope.ChildTypeTrv = '';
    $scope.InfantTypeTrv = '';
    $scope.Tripmode = '1';
    
    //End Add For Traveler List

    $scope.isAdvanceSearch = false;
    $scope.Airports = [];
    $scope.SelectedAirport = null;
    $scope.AirportsName = '';
    $scope.ToAirportsName = '';
    $scope.AirportsCode = '';
    $scope.ToAirportsCode = '';
    $scope.ToAirportName = null;
    $scope.FromDate = '';
    $scope.ToDate = '';
    $scope.TravellType = 'Y';
    $scope.objListing =
        {
            travels:
                [
                    {
                        AirTravellers:
                        {
                            AirTotal: 1,
                            AirAdult: 1,
                            AirChildren: 0,
                            AirInfant: 0

                        }
                    }
                ],
        };
    $scope.formdisplay = true;
    $scope.airlineList = new Array();

    $scope.tripType = '1';
    $scope.getAirport = function (val) {
        return $http.get('/Merchant/MerchantFlightBooking/GetAllAirports', {
            params: {
                req: val,
            }
        }).then(function (response) {
            return response.data;
        });
    };

    ////event fires when click on textbox  From AirportName
    //$scope.SelectedAirport = function (selected) {        
    //    if (selected) {            
    //        $scope.SelectedAirport = selected.originalObject;            
    //        //alert($scope.SelectedAirport);
    //    }
    //}
    //// To airport Name
    //$scope.ToAirportName = function (selected) {
    //    if (selected) {            
    //        $scope.ToAirportName = selected.originalObject;
    //        //alert($scope.ToAirportName);
    //    }
    //}
    ////Gets data from the Database
    //$http({
    //    method: 'GET',
    //    url: '/MerchantFlightDetails/GetAllAirportName'
    //}).then(function (data) {
    //    $scope.Airports = data.data;
    //}, function () {
    //    // alert('Error');
    //})
    //Airport.TravellType
    $scope.Airport = { Tripmode : 1 };
    $scope.SerachFlights = function () {
        debugger;
        var Tripmode = $scope.Airport.Tripmode;
        if (Tripmode != '3') {
            var date_to = "";
            $scope.formdisplay = true;
            
            var From_DAte = $scope.FromDate;
            //var From_DAte = DeptDate;
            //var FromAirportsName = $scope.SelectedAirport.CITYNAME;
            //var FromCityCode = $scope.SelectedAirport.CITYCODE;
            //var TOAirportName = $scope.ToAirportName.CITYNAME;
            //var TOAirportCode = $scope.ToAirportName.CITYCODE;
            var FromAirportsName = $scope.fromAirportDetails.CITYNAME;
            var FromCityCode = $scope.fromAirportDetails.CITYCODE;
            var TOAirportName = $scope.toAirportDetails.CITYNAME;
            var TOAirportCode = $scope.toAirportDetails.CITYCODE;

            //var FromDate = $scope.FromDate;
            const DeptDate = moment($scope.FromDate).format('YYYY-MM-DD');
            var FromDate = DeptDate;
            if (Tripmode == 1) {
                date_to = 0;
            }
            else if (Tripmode == 2) {
                const RetDate = moment($scope.ToDate).format('YYYY-MM-DD');
                //date_to = $scope.ToDate;
                date_to = RetDate;
            }
            var ToDate = date_to;
            var TravelType = $scope.TravellType;
            //var Adult = $scope.objListing.travels[0].AirTravellers.AirAdult;
            //var Child = $scope.objListing.travels[0].AirTravellers.AirChildren;
            //var Infant = $scope.objListing.travels[0].AirTravellers.AirInfant;
            var Adult = $scope.adultTypeTrv;
            var Child = $scope.ChildTypeTrv;
            var Infant = $scope.InfantTypeTrv;
            var data = {
                Tripmode: Tripmode,
                FromAirportsName: FromAirportsName,
                FromCityCode: FromCityCode,
                TOAirportName: TOAirportName,
                TOAirportCode: TOAirportCode,
                FromDate: FromDate,
                ToDate: ToDate,
                TravelType: TravelType,
                Adult: Adult,
                Child: Child,
                Infant: Infant
            };
            $('#progress').show();
            //var url = "/Merchant/MerchantFlightDetails/GetSearchResult/" + Tripmode + "/" + FromCityCode + "/" + TOAirportCode + "/" + FromDate + "/" + ToDate + "/" + TravelType + "/" + Adult + "/" + Child + "/" + Infant;
            if (Tripmode == 1) {
                var url = "/Merchant/MerchantFlightDetails/GetSearchResult?Tripmode=" + Tripmode + "&FromCityCode=" + FromCityCode + "&TOAirportCode=" + TOAirportCode + "&FromDate=" + FromDate + "&ToDate=" + ToDate + "&TravelType=" + TravelType + "&Adult=" + Adult + "&Child=" + Child + "&Infant=" + Infant;
                window.location.href = url;
                
            }
            else if (Tripmode == 2) {
                var url = "/Merchant/MerchantFlightDetails/ReturnFlightlist?Tripmode=" + Tripmode + "&FromCityCode=" + FromCityCode + "&TOAirportCode=" + TOAirportCode + "&FromDate=" + FromDate + "&ToDate=" + ToDate + "&TravelType=" + TravelType + "&Adult=" + Adult + "&Child=" + Child + "&Infant=" + Infant;
                window.location.href = url;
            }

           
        }
        else {
            var MulticityValue = $scope.AddMulticitySearchInfo;
            var multicitydetails = null;
            var CityInfo = '';
            var src = null;
            var dest = null;
            var date_val = null;
            var addvalue = '';
            for (var j = 0; j < MulticityValue.length; j++) {
                src = MulticityValue[j].Source;
                dest = MulticityValue[j].Destination;
                date_val = MulticityValue[j].DateofJourney;
                var dateconvert = $filter('date')(new Date(date_val), "ddMMyyyy");
                //addvalue = src + "-" + dest + "-" + dateconvert;
                addvalue = src + "^" + dest + "^" + dateconvert;
                if (j == 0) {
                    CityInfo = addvalue;
                }
                else {
                    CityInfo = CityInfo+"-"+ addvalue;
                }
                
                addvalue = '';
            }

            //CityInfo = CityInfo;
            //angular.forEach(MulticityValue, function (val, key) {
            //    src = val.Source;
            //    dest = val.Destination;
            //    date_val = val.DateofJourney;
            //    CityInfo = val.Source + "-" + val.Destination + "-" + val.DateofJourney;
                
            //});
            multicitydetails = CityInfo;
            var date_to = "";
            $scope.formdisplay = true;

            //var FromAirportsName = $scope.SelectedAirport.CITYNAME;
            //var FromCityCode = $scope.SelectedAirport.CITYCODE;
            //var TOAirportName = $scope.ToAirportName.CITYNAME;
            //var TOAirportCode = $scope.ToAirportName.CITYCODE;

            //var FromDate = $scope.FromDate;
            //if (Tripmode == 1) {
            //    date_to = 0;
            //}
            //else if (Tripmode == 2) {
            //    date_to = $scope.ToDate;
            //}
            //var ToDate = date_to;
            var TravelType = $scope.TravellType;
            var Adult = $scope.objListing.travels[0].AirTravellers.AirAdult;
            var Child = $scope.objListing.travels[0].AirTravellers.AirChildren;
            var Infant = $scope.objListing.travels[0].AirTravellers.AirInfant;
            //var data = {
            //    Tripmode: Tripmode,
            //    FromAirportsName: FromAirportsName,
            //    FromCityCode: FromCityCode,
            //    TOAirportName: TOAirportName,
            //    TOAirportCode: TOAirportCode,
            //    FromDate: FromDate,
            //    ToDate: ToDate,
            //    TravelType: TravelType,
            //    Adult: Adult,
            //    Child: Child,
            //    Infant: Infant
            //};
            $('#progress').show();
            var url = "/Merchant/MerchantFlightDetails/GetMultiCitySearch/" + Tripmode + "/" + multicitydetails + "/" + TravelType + "/" + Adult + "/" + Child + "/" + Infant;
            window.location.href = url;
        }
        
        
    };

    //// search function to match full text
    //$scope.localSearch = function (str, Airports) {
    //    debugger;
    //    var matches = [];
    //    Airports.forEach(function (item) {
    //        var CITYNAME = item.CITYNAME;
    //        if ((item.CITYNAME.toLowerCase().indexOf(str.toString().toLowerCase()) >= 0)) {
    //            matches.push(item);
    //        }
    //    });
    //    return matches;
    //};

    $scope.LoadFlightSearch = function (Tripmode, FromCityCode, TOAirportCode, FromDate, ToDate, TravelType, Adult, Child, Infant) {
        debugger;
        $('#progress').show();
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
        //if (Tripmode == 1) {
        //    $scope.TripModeValue = 1;
        //}
        //else if (Tripmode == 2)
        //{
        //    $scope.TripModeValue = 2;
        //}
        var data = {
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

        $http({
            url: '/Merchant/MerchantFlightDetails/SerachFlight',
            method: "POST",
            data: data,
        }).then(function (response) {
            debugger;            
            console.log(response);
            //$scope.TripMode = Tripmode;
            //$scope.AdultCount = Adult;
            //$scope.ChildCount = Child;
            //$scope.InfantCount = Infant;
            // var searchresponse = JSON.parse(response.data);            
            var FlightResponse = JSON.parse(response.data);
            var info = FlightResponse.GetFlightAvailibilityResponse;
            var FlightSearchDetails = FlightResponse.GetFlightAvailibilityResponse.FlightDetails;

           

            $scope.filterhtml = true;
            if (Tripmode == 1) {
                $scope.flightsearchResult = FlightSearchDetails;
                console.log(FlightSearchDetails);
                //var myObj = { Time: new Date(), Token: FlightSearchDetails.TrackNo };
                var myObj = { Time: new Date(), Token: FlightSearchDetails };
                localStorage.setItem('searchResult', null);
                localStorage.setItem('SearchTraceDetails', JSON.stringify(myObj));
                localStorage.setItem('searchResult', JSON.stringify(FlightSearchDetails));
            }
            else if (Tripmode == 2) {            
                $scope.flightsearchResult = FlightSearchDetails;

                $scope.deptureFlight = FlightSearchDetails.filter(x => x.SrNo.charAt(x.SrNo.length - 1) === 'O');
                $scope.returnFlight = FlightSearchDetails.filter(x => x.SrNo.charAt(x.SrNo.length - 1) === 'R');
                var myObj = { Time: new Date(), Token: FlightSearchDetails.TrackNo };
                localStorage.setItem('searchResult', null);
                localStorage.setItem('SearchTraceDetails', JSON.stringify(myObj));
                localStorage.setItem('searchResult', JSON.stringify(FlightSearchDetails));
                console.log($scope.deptureFlight);
                console.log($scope.returnFlight);
            }
            
            $('#progress').hide();
            $scope.formdisplay = false;
            if (Tripmode == 1) {
                $scope.TripModeValue = 1;
            }
            else if (Tripmode == 2) {
                $scope.TripModeValue = 2;
            }
            console.log($scope.flightsearchResult);
        },
        function (response) {
            console.log(response.data);
        });
    };

    $scope.loadMore = function () {
        $scope.numberToDisplay = $scope.numberToDisplay + 10;
    }
    $scope.FilterByStopsNAN = function (a) {

        if (a.Stops =="0")
            return true;
        else
            return false;

    }
    $scope.FilterByStops = function (a) {

        if (a.Stops == "1")
            return true;
        else
            return false;

    }
    $scope.ReturnOneFilterByStopsNAN = function (a) {

        if (a.SrNo.slice(SrNo.length - 1) == "O" )
            return true;
        else
            return false;

    }
    $scope.ReturntwoFilterByStopsNAN = function (a) {

        if (a.SrNo.slice(SrNo.length - 1) == "R")
            return true;
        else
            return false;

    }



    $scope.MultiCitySearch = function (TravelType, Adult, Child, Infant, Tripmode, MulticityInfo) {
        debugger;
        $('#progress').show();
        $scope.formdisplay = true;
        var Tripmode = Tripmode;
        var multiinfo = MulticityInfo;
        var TravelType = TravelType;
        var Adult = Adult;
        var Child = Child;
        var Infant = Infant;
        $scope.TripMode = Tripmode;
        $scope.AdultCount = Adult;
        $scope.ChildCount = Child;
        $scope.InfantCount = Infant;
        
        var data = {
            Tripmode: Tripmode,
            TravelType: TravelType,
            Adult: Adult,
            Child: Child,
            Infant: Infant,
            multiinfo: multiinfo
        };

        $http({
            url: '/Merchant/MerchantFlightBooking/MulticitySearch',
            method: "POST",
            //data: data,
            data: {
                Tripmode: Tripmode,
                TravelType: TravelType,
                Adult: Adult,
                Child: Child,
                Infant: Infant,
                multiinfo: multiinfo
            }
        }).then(function (response) {
            debugger;
            var searchresponse = JSON.parse(response.data);
            if (searchresponse.Response.ResponseStatus == 1) {
                if (window.localStorage) {
                    var myObj = { Time: new Date(), Token: searchresponse.Response.TraceId };
                    localStorage.setItem('searchResult', null);
                    localStorage.setItem('SearchTraceDetails', JSON.stringify(myObj));
                    localStorage.setItem('searchResult', JSON.stringify(searchresponse.Response.Results));
                }
                $scope.filterhtml = true;
                $scope.flightsearchResult = searchresponse.Response.Results;
                //$scope.airlines = new Array();
                //angular.forEach($scope.flightsearchResult[0], function (value, key) {
                //    var res = $filter('filter')($scope.airlines, { code: value.Segments[0][0].Airline.AirlineCode });
                //    if (res.length == 0) {
                //        var obj = { name: value.Segments[0][0].Airline.AirlineName, code: value.Segments[0][0].Airline.AirlineCode };
                //        $scope.airlines.push(obj);
                //    }
                //});
                $('#progress').hide();
                $scope.formdisplay = false;
            }
            else {
                bootbox.alert({
                    size: "small",
                    //message: "Booking has confirmed in the name of " + BookingInfo.Response.Response.FlightItinerary.Passenger[0].FirstName + " " + BookingInfo.Response.Response.FlightItinerary.Passenger[0].LastName +" .This is PNR " + BookingInfo.Response.Response.PNR + " and BookingId " + BookingInfo.Response.Response.BookingId+"",
                    message: searchresponse.Response.Error.ErrorMessage,
                    backdrop: true,
                    callback: function () {
                        var URL = "/Merchant/MerchantFlightBooking/Index";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
            }

            
            //if (Tripmode == 1) {
            //    $scope.TripModeValue = 1;
            //}
            //else if (Tripmode == 2) {
            //    $scope.TripModeValue = 2;
            //}
        },
            function (response) {
                console.log(response.data);
            });
    };





    $scope.GerSearchResult = function () {
        $scope.formdisplay = true;
        var Tripmode = $scope.Airport.Tripmode;
        var FromAirportsName = $scope.SelectedAirport.CITYNAME;
        var FromCityCode = $scope.SelectedAirport.CITYCODE;
        var TOAirportName = $scope.ToAirportName.CITYNAME;
        var TOAirportCode = $scope.ToAirportName.CITYCODE;
        var FromDate = $scope.FromDate;
        var ToDate = $scope.ToDate;
        var TravelType = $scope.TravellType;
        var Adult = $scope.objListing.travels[0].AirTravellers.AirAdult;
        var Child = $scope.objListing.travels[0].AirTravellers.AirChildren;
        var Infant = $scope.objListing.travels[0].AirTravellers.AirInfant;
        $scope.AdultCount = Adult;
        $scope.ChildCount = Child;
        $scope.InfantCount = Infant;
        $scope.TripmodeValue = Tripmode;

        var data = {
            Tripmode: Tripmode,
            FromAirportsName: FromAirportsName,
            FromCityCode: FromCityCode,
            TOAirportName: TOAirportName,
            TOAirportCode: TOAirportCode,
            FromDate: FromDate,
            ToDate: ToDate,
            TravelType: TravelType,
            Adult: Adult,
            Child: Child,
            Infant: Infant
        };
        $('#progress').show();
        $http({
            url: '/Merchant/MerchantFlightBooking/SerachFlight',
            method: "POST",
            data: data,
        }).then(function (response) {
            var searchresponse = JSON.parse(response.data);
            if (window.localStorage) {
                var myObj = { Time: new Date(), Token: searchresponse.Response.TraceId };
                localStorage.setItem('SearchTraceDetails', JSON.stringify(myObj));
            }            
            if (Tripmode == 2) {
                $scope.Tripmode = 2;
            }
            else {
                $scope.Tripmode = 1;
            }            
            $scope.flightsearchResult = searchresponse.Response.Results;
            $scope.formdisplay = false;
            $scope.RbtSourceDetails.checked = false;
            $scope.RbtDestinationDetails.checked = false;
            
            console.log($scope.AdultCount, $scope.ChildCount, $scope.InfantCount);
            $('#progress').hide();
            console.log($scope.flightsearchResult);
            console.log(JSON.stringify(searchresponse));
        },
            function (response) {
                console.log(response.data);
            });
    };

    $scope.setDate = function () {
        $scope.FromDate = $("#datepickerFromDate").val();
        var minDate = new Date($scope.FromDate.date.valueOf());
        $('#datepickerToDate').datepicker('setStartDate', minDate);
    };
    $scope.setToDate = function () {
        debugger;
        var fromdate = $scope.FromDate;
        $scope.ToDate = $("#datepickerToDate").val();
    };
    $scope.setMultiCityDate = function () {
        var fromdate = $scope.FromDate;
        $scope.DateofJourney = $("#datepickerFromDate1").val();
    };

    $scope.advnceSearch = function ($item) {
        console.log($item);
        var data = {
            "adultCount": null,
            "childCount": null,
            "infantCount": null,
            "tokenId": null,
            "traceId": null,
            "resultIndex": null,
            "source": 0,
            "isLCC": null,
            "isRefundable": null,
            "airlineRemark": null,
            "tripIndicator": 0,
            "segmentIndicator": 0,
            "airlineCode": null,
            "airlineName": null,
            "flightNumber": null,
            "fareClass": null,
            "operatingCarrier": null
        };

        $http({
            url: '/Merchant/MerchantFlightBooking/AdvanceSerachFlight',
            method: "POST",
            data: data,
        }).then(function (response) {
            var searchresponse = JSON.parse(response.data);
            //if (window.localStorage) {
            //    let myObj = { Time: new Date(), Token: searchresponse.Response.TraceId };
            //    localStorage.setItem('SearchTraceDetails', JSON.stringify(myObj));
            //}

            $scope.flightsearchResult = searchresponse.Response.Results;
            $scope.formdisplay = false;
        },
            function (response) {
                console.log(response.data);
            });

        alert(JSON.stringify($item));
    };

    
    $scope.getMultiSearchDetailsOfFlight = function (ResultIndex, TripMode, Passenger) {
        debugger;
        var Tracevalue = JSON.parse(window.localStorage.getItem("SearchTraceDetails"));
        var timevalue = new Date(Tracevalue.Time);
        var TraceId = Tracevalue.Token;
        const currDate = new Date();
        const oldDate = timevalue;

        var list = (currDate - oldDate) / 60000;
        if (list <= 15) {
            window.location.href = '/Merchant/MerchantFlightBooking/MultiCityBooking?BookingValue=' + ResultIndex + '&token=' + TraceId + '&Passenger=' + Passenger + '&TripMode=' + TripMode;
        }
        else {
            bootbox.alert({
                message: "Session is expired.Please search again",
                backdrop: true
            });
        }
    };


    //Adult, Children, Infant, TrackNo, TripMode
    //$scope.RoundtripgetFlightDetails = function (item, Passenger, IsLCC, TripMode) {
    $scope.RoundtripgetFlightDetails = function (Adult, Children, Infant, TrackNo, TripMode) {
        debugger;
        var Tracevalue = JSON.parse(window.localStorage.getItem("SearchTraceDetails"));
        var timevalue = new Date(Tracevalue.Time);
        var TraceId = Tracevalue.Token;
        const currDate = new Date();
        const oldDate = timevalue;

        var list = (currDate - oldDate) / 60000;
        if (list <= 15) {
            //window.location.href = '/Merchant/MerchantFlightBooking/FlightBooking?BookingValue=' + item + '&token=' + TraceId + '&Passenger=' + Passenger + '&TripMode=' + TripMode + '&IsLCC=' + IsLCC;
            window.location.href = '/Merchant/MerchantFlightDetails/FlightBookingDetails?TrackNo=' + TrackNo + '&PsgnAdult=' + Adult + '&PsgnChildren=' + Children + '&PsgnInfant=' + Infant + '&TripMode=' + TripMode;
        }
        else {
            bootbox.alert({
                message: "Session is expired.Please search again",
                backdrop: true
            });
        }
    };

    $scope.timeConvert = function (n) {
        var minutes = n % 60
        var hours = (n - minutes) / 60
        return hours + " hr " + minutes + " m";
    }
    $scope.selectDestimationSource = function (TrackNo, FromAirportCode, DepTime, Via, TotalDur, Stops, TOAirportCode, ArivTime, DepDate, TotalAmount, SrNo,FlightNo) {
        debugger;          
        //var FlightSelect = $scope.deptureFlight[$index];
        $scope.selectval = true;
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
     
    }
    $scope.selectReturnFlightSource = function (TrackNo, FromAirportCode, DepTime, Via, TotalDur, Stops, TOAirportCode, ArivTime, DepDate, TotalAmount, SrNo, FlightNo){
        debugger;
        //var FlightSelect = $scope.flightsearchResult.FlightDetails[$index];
        //var FlightSelect = $scope.returnFlight[$index];
        $scope.selectval = true;
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
        
    }

    $scope.selectSource = function ($index) {
        debugger;
        //$('#rbtSourceId' + $index + '').prop("checked", true).trigger("click");
        //var FlightSelect = $scope.flightsearchResult[0][$index];
        //var FlightSelect = $scope.flightsearchResult.FlightDetails[$index];        
        var FlightSelect = $scope.deptureFlight[$index];        
        $scope.selectval = true;
        $scope.OriginResultIndex = FlightSelect.SrNo;
        $scope.FlightName = FlightSelect.AirlineCode;
        $scope.FlightCode = FlightSelect.FlightNo;
        $scope.FlightNo = FlightSelect.FlightNo;
        $scope.Duration = FlightSelect.FlightTime;
        $scope.FromStops = FlightSelect.Stops;
        $scope.OriginAirportCode = FlightSelect.FromAirportCode;
        $scope.Departtime = FlightSelect.DepTime;
        $scope.DepartDate = FlightSelect.DepDate;
        $scope.Reachedtime = FlightSelect.ArrTime;
        $scope.ReachedDate = FlightSelect.DepDate;
        $scope.DestinationAirportcode = FlightSelect.ToAirportCode;
        $scope.DeparturePublishedFare = FlightSelect.TotalAmount;
        $scope.SourceISLCC = FlightSelect.TrackNo;
        $scope.FlightPrice = FlightSelect.TotalAmount;
        $scope.FlightTaxPrice = FlightSelect.TaxAmount;
        $scope.FlightTrackNo = FlightSelect.TrackNo;
        //$scope.OriginResultIndex = FlightSelect.ResultIndex;
        //$scope.FlightName = FlightSelect.Segments[0][0].Airline.AirlineName;
        //$scope.FlightCode = FlightSelect.Segments[0][0].Airline.AirlineCode;
        //$scope.FlightNo = FlightSelect.Segments[0][0].Airline.FlightNumber;
        //$scope.Duration = FlightSelect.Segments[0][0].Origin.DepTime;
        //$scope.OriginAirportCode = FlightSelect.Segments[0][0].Origin.Airport.AirportCode;
        //$scope.Reachedtime = FlightSelect.Segments[0][0].Destination.ArrTime;
        //$scope.DestinationAirportcode = FlightSelect.Segments[0][0].Destination.Airport.AirportCode;
        //$scope.DeparturePublishedFare = FlightSelect.Fare.PublishedFare;       
        //$scope.SourceISLCC = FlightSelect.IsLCC;
    }

    $scope.selectDestination = function ($index) {
        debugger;
        //var FlightSelect = $scope.flightsearchResult.FlightDetails[$index];
        var FlightSelect = $scope.returnFlight[$index];
        $scope.selectval = true;
        $scope.DesOriginResultIndex = FlightSelect.SrNo;
        $scope.DesFlightName = FlightSelect.AirlineCode;
        $scope.DesFlightCode = FlightSelect.FlightNo;
        $scope.DesFlightNo = FlightSelect.FlightNo;
        $scope.DesDuration = FlightSelect.FlightTime;
        $scope.DesFromStops = FlightSelect.Stops;
        $scope.DesOriginAirportCode = FlightSelect.FromAirportCode;
        $scope.DesDeparttime = FlightSelect.DepTime;
        $scope.DesDepartDate = FlightSelect.DepDate;
        $scope.DesReachedtime = FlightSelect.ArrTime;
        $scope.DesReachedDate = FlightSelect.DepDate;
        $scope.DesDestinationAirportcode = FlightSelect.ToAirportCode;
        $scope.DesDeparturePublishedFare = FlightSelect.TotalAmount;
        $scope.DesSourceISLCC = FlightSelect.TrackNo;
        $scope.DesFlightPrice = FlightSelect.TotalAmount;
        $scope.DesFlightTaxPrice = FlightSelect.TaxAmount;
        $scope.DesFlightTrackNo = FlightSelect.TrackNo;
        //$('#rbtDestinationId' + $index + '').prop("checked", true).trigger("click");
        //var FlightSelect = $scope.flightsearchResult[1][$index];
        //$scope.DestinationResultIndex = FlightSelect.ResultIndex;
        //$scope.DestinationFlightName = FlightSelect.Segments[0][0].Airline.AirlineName;
        //$scope.DestinationFlightCode = FlightSelect.Segments[0][0].Airline.AirlineCode;
        //$scope.DestinationFlightNo = FlightSelect.Segments[0][0].Airline.FlightNumber;
        //$scope.DeptTime = FlightSelect.Segments[0][0].Origin.DepTime;
        //$scope.DeptSourceAirport = FlightSelect.Segments[0][0].Origin.Airport.AirportCode;
        //$scope.DestArrivalTime = FlightSelect.Segments[0][0].Destination.ArrTime;
        //$scope.DestArrivalairportcode = FlightSelect.Segments[0][0].Destination.Airport.AirportCode;
        //$scope.DepartFarePrice = FlightSelect.Fare.PublishedFare;
        //$scope.GrandTotal = ($scope.DeparturePublishedFare) + ($scope.DepartFarePrice);
        //$scope.DestinationISLCC = FlightSelect.IsLCC;
    }

    $scope.uotboundTime = {
        morning: false,
        afternoon: false,
        evening: false,
        night: false
    };

    $scope.parseFloat = function (value) {
        return parseFloat(value);
    }

    $scope.parseNumber = function (value) {
        return parseInt(value);
    }
    

    $scope.filteroutboundtime = function () {
        if ($scope.TripModeValue == 1) {
            var Tracevalue = JSON.parse(window.localStorage.getItem("searchResult"));
            var searchResult = Tracevalue[0];

            var res = new Array();

            $scope.flightsearchResult[0] = new Array();
            if (!$scope.uotboundTime.morning && !$scope.uotboundTime.afternoon && !$scope.uotboundTime.evening && !$scope.uotboundTime.night) {
                var filteres = new Array();
                var airlineslist = $scope.selectedAirlines;
                angular.forEach(searchResult, function (value, key) {
                    var isinarrey = false;
                    angular.forEach(airlineslist, function (value2, key) {
                        if (value.AirlineCode == value2) {
                            isinarrey = true;
                        }
                    });
                    if (airlineslist.length > 0) {
                        if (isinarrey) {
                            filteres.push(value);
                        }
                    }
                    else {
                        filteres.push(value);
                    }
                });
                $scope.flightsearchResult[0] = filteres;
            }
            else {
                angular.forEach(searchResult, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.morning) {
                        if (040000 < time && time < 110000) {
                            res.push(value);
                        }
                    }
                });
                angular.forEach(searchResult, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.afternoon) {
                        if (110000 < time && time < 160000) {
                            res.push(value);
                        }
                    }
                });
                angular.forEach(searchResult, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.evening) {
                        if (160000 < time && time < 210000) {
                            res.push(value);
                        }
                    }
                });
                angular.forEach(searchResult, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.night) {
                        if (210000 < time && time < 240000) {
                            res.push(value);
                        }
                        if (000000 < time && time < 040000) {
                            res.push(value);
                        }
                    }
                });
                var filteres = new Array();
                var airlineslist = $scope.selectedAirlines;
                angular.forEach(res, function (value, key) {
                    var isinarrey = false;
                    angular.forEach(airlineslist, function (value2, key) {
                        if (value.AirlineCode == value2) {
                            isinarrey = true;
                        }
                    });
                    if (airlineslist.length > 0) {
                        if (isinarrey) {
                            filteres.push(value);
                        }
                    }
                    else {
                        filteres.push(value);
                    }
                });

                $scope.flightsearchResult[0] = filteres;
            }
        }
        else {


            var Tracevalue = JSON.parse(window.localStorage.getItem("searchResult"));
            var searchResult1 = Tracevalue[0];
            var searchResult2 = Tracevalue[1];

            var res1 = new Array();
            var res2 = new Array();

            $scope.flightsearchResult[0] = new Array();
            if (!$scope.uotboundTime.morning && !$scope.uotboundTime.afternoon && !$scope.uotboundTime.evening && !$scope.uotboundTime.night) {
                var filteres = new Array();
                var airlineslist = $scope.selectedAirlines;
                angular.forEach(searchResult1, function (value, key) {
                    var isinarrey = false;
                    angular.forEach(airlineslist, function (value2, key) {
                        if (value.AirlineCode == value2) {
                            isinarrey = true;
                        }
                    });
                    if (airlineslist.length > 0) {
                        if (isinarrey) {
                            filteres.push(value);
                        }
                    }
                    else {
                        filteres.push(value);
                    }
                });
                $scope.flightsearchResult[0] = filteres;
            }
            else {
                angular.forEach(searchResult1, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.morning) {
                        if (040000 < time && time < 110000) {
                            res1.push(value);
                        }
                    }
                });
                angular.forEach(searchResult1, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.afternoon) {
                        if (110000 < time && time < 160000) {
                            res1.push(value);
                        }
                    }
                });
                angular.forEach(searchResult1, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.evening) {
                        if (160000 < time && time < 210000) {
                            res1.push(value);
                        }
                    }
                });
                angular.forEach(searchResult1, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.night) {
                        if (210000 < time && time < 240000) {
                            res1.push(value);
                        }
                        if (000000 < time && time < 040000) {
                            res1.push(value);
                        }
                    }
                });
                var filteres1 = new Array();
                var airlineslist = $scope.selectedAirlines;
                angular.forEach(res1, function (value, key) {
                    var isinarrey = false;
                    angular.forEach(airlineslist, function (value2, key) {
                        if (value.AirlineCode == value2) {
                            isinarrey = true;
                        }
                    });
                    if (airlineslist.length > 0) {
                        if (isinarrey) {
                            filteres1.push(value);
                        }
                    }
                    else {
                        filteres1.push(value);
                    }
                });

                $scope.flightsearchResult[0] = filteres1;
            }


            $scope.flightsearchResult[1] = new Array();
            if (!$scope.uotboundTime.morning && !$scope.uotboundTime.afternoon && !$scope.uotboundTime.evening && !$scope.uotboundTime.night) {
                var filteres = new Array();
                var airlineslist = $scope.selectedAirlines;
                angular.forEach(searchResult2, function (value, key) {
                    var isinarrey = false;
                    angular.forEach(airlineslist, function (value2, key) {
                        if (value.AirlineCode == value2) {
                            isinarrey = true;
                        }
                    });
                    if (airlineslist.length > 0) {
                        if (isinarrey) {
                            filteres.push(value);
                        }
                    }
                    else {
                        filteres.push(value);
                    }
                });
                $scope.flightsearchResult[1] = filteres;
            }
            else {
                angular.forEach(searchResult1, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.morning) {
                        if (040000 < time && time < 110000) {
                            res2.push(value);
                        }
                    }
                });
                angular.forEach(searchResult1, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.afternoon) {
                        if (110000 < time && time < 160000) {
                            res2.push(value);
                        }
                    }
                });
                angular.forEach(searchResult1, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.evening) {
                        if (160000 < time && time < 210000) {
                            res2.push(value);
                        }
                    }
                });
                angular.forEach(searchResult1, function (value, key) {
                    var date = new Date(value.Segments[0][0].Origin.DepTime);
                    var time = parseInt($filter('date')(date, 'HHmmss'));
                    if ($scope.uotboundTime.night) {
                        if (210000 < time && time < 240000) {
                            res2.push(value);
                        }
                        if (000000 < time && time < 040000) {
                            res2.push(value);
                        }
                    }
                });
                var filteres1 = new Array();
                var airlineslist = $scope.selectedAirlines;
                angular.forEach(res2, function (value, key) {
                    var isinarrey = false;
                    angular.forEach(airlineslist, function (value2, key) {
                        if (value.AirlineCode == value2) {
                            isinarrey = true;
                        }
                    });
                    if (airlineslist.length > 0) {
                        if (isinarrey) {
                            filteres1.push(value);
                        }
                    }
                    else {
                        filteres1.push(value);
                    }
                });

                $scope.flightsearchResult[1] = filteres1;
            }
        }
    }

});
//app.directive('popoverpassengers', function ($compile) {
//    return {
//        restrict: 'A',
//        link: function (scope, elem) {

//            var content = $("#popover-content-popover-passengers").html();
//            var compileContent = $compile(content)(scope);
//            var options = {
//                content: compileContent,
//                html: true,
//            };

//            $(elem).popover(options);
//        }
//    }
//})
