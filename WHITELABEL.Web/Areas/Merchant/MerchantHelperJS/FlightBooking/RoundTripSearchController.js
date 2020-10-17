app.controller('RoundTripSearchController', ['FlightServices', '$scope', '$http', '$window', '$filter', function (FlightServices, $scope, $http, $window, $filter) {
    $scope.displayNetAmount = false;
    $scope.additionalAddedAmount = parseFloat(document.getElementById('AIRADDITIONALAMOUNT').value);
    $scope.InternationaladditionalAddedAmount = parseFloat(document.getElementById('INTERAIRADDITIONALAMOUNT').value);

    $scope.getFloatNumber = function (n) {
        if (n) {
            return parseFloat(n);
        }
        return 0;
    }

    $scope.lower_price_bound = 0;
    $scope.upper_price_bound = 1000;
    $scope.min = 0;
    $scope.max = 1000;

    $scope.Return_lower_price_bound = 0;
    $scope.Return_upper_price_bound = 1000;
    $scope.Return_min = 0;
    $scope.Return_max = 1000;



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

    $scope.outBoundAirlinesList = [];
    $scope.inBoundAirlinesList = [];

    $scope.selectDeptTrackNo = '';
    $scope.selectReturnTrackNo = '';



    $scope.$watch('outBoundAirlinesList|filter:{selected:true}', function (nv) {
        $scope.filterData.airlines = nv.map(function (item) {
            return item.code;
        });
    }, true);


    $scope.$watch('inBoundAirlinesList|filter:{selected:true}', function (nv) {
        $scope.filterRetrunData.airlines = nv.map(function (item) {
            return item.code;
        });
    }, true);


    $scope.filterDeptFlightData = function (item) {

        let returnValue = false;

        let amount = Math.round(item[0].TotalAmount);
        returnValue = Math.round(amount) >= ($scope.lower_price_bound - $scope.additionalAddedAmount) && Math.round(amount) <= ($scope.upper_price_bound - $scope.additionalAddedAmount);

        if (($scope.filterData.timeSlots.EarlyMorning
            || $scope.filterData.timeSlots.Morning
            || $scope.filterData.timeSlots.MidDay
            || $scope.filterData.timeSlots.Evening
            || $scope.filterData.timeSlots.Night) && returnValue) {

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
        returnValue = Math.round(amount) >= $scope.Return_lower_price_bound && Math.round(amount) <= $scope.Return_upper_price_bound;

        if (($scope.filterRetrunData.timeSlots.EarlyMorning
            || $scope.filterRetrunData.timeSlots.Morning
            || $scope.filterRetrunData.timeSlots.MidDay
            || $scope.filterRetrunData.timeSlots.Evening
            || $scope.filterRetrunData.timeSlots.Night) && returnValue) {

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

        if ($scope.filterRetrunData.stops != null && returnValue) {
            returnValue = item[0].Stops == $scope.filterRetrunData.stops;
        }

        if ($scope.filterData.refundable != null && returnValue) {
            returnValue = item[0].FareType == ($scope.filterData.refundable == 'true' ? 'R' : 'N');
        }

        if ($scope.filterRetrunData.airlines.length > 0 && returnValue) {
            returnValue = $scope.filterRetrunData.airlines.includes(item[0].AirlineCode);
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
      
        $scope.FromCityCodeVal = FromCityCode;
        $scope.ToDistination = TOAirportCode;
        $scope.TripMode = Tripmode;
        $scope.AdultCount = Adult;
        $scope.ChildCount = Child;
        $scope.InfantCount = Infant;

        const data = {
            Tripmode: Tripmode,
            FromAirportsName: FromCityCode,
            FromCityCode: FromCityCode,
            TOAirportName: TOAirportCode,
            TOAirportCode: TOAirportCode,
            FromDate: FromDate,
            ToDate: ToDate,
            TravelType: TravelType,
            Adult: Adult,
            Child: Child,
            Infant: Infant
        };

        localStorage.setItem('SEARCH_FLIGHT_DATA', JSON.stringify(data));

        $scope.loadDeptureFlightSearch(Tripmode, FromCityCode, TOAirportCode, FromDate, ToDate, TravelType, Adult, Child, Infant);
        $scope.loadReturnFlightSearch(Tripmode, FromCityCode, TOAirportCode, FromDate, ToDate, TravelType, Adult, Child, Infant);
    }

    // #region Load detpture flight 
    $scope.loadDeptureFlightSearch = function (Tripmode, FromCityCode, TOAirportCode, FromDate, ToDate, TravelType, Adult, Child, Infant) {


        let data = {
            Tripmode: Tripmode,
            FromAirportsName: FromCityCode,
            FromCityCode: FromCityCode,
            TOAirportName: TOAirportCode,
            TOAirportCode: TOAirportCode,
            FromDate: FromDate,
            ToDate: ToDate,
            TravelType: TravelType,
            Adult: Adult,
            Child: Child,
            Infant: Infant
        };

        localStorage.setItem('SEARCH_FLIGHT_DATA', JSON.stringify(data));
        data.Tripmode = 1;
        const service = FlightServices.getFlightSingleSearchDetails(data);
        service.then(function (response) {

            const data = response.data;
            const FlightResponse = JSON.parse(data);

            //=========================== Store All Flight Information =============================//
            $scope.deptureFlightDetails = FlightResponse.GetFlightAvailibilityResponse.FlightDetails;
            $scope.deptureFareDetails = FlightResponse.GetFlightAvailibilityResponse.FareDetails;
            $scope.deptureAirlineList = FlightResponse.GetFlightAvailibilityResponse.AirlineList;
            $scope.deptureAirportList = FlightResponse.GetFlightAvailibilityResponse.AirportList;


            $scope.outBoundAirlinesList = $scope.deptureAirlineList.map(item => {
                const container = {};
                container.name = item.AirlineName;
                container.code = item.AirlineCode;
                container.selected = false;
                return container;
            });


            //================= Group by track number =============================//
            let objdeptureFlight = $filter('groupBy')($scope.deptureFlightDetails, 'TrackNo');

            //================= Store a copy for feature uses =======================//
            $scope.deptureFlightsearchResultTrackNo = Object.keys(objdeptureFlight).map(function (key) {
                return objdeptureFlight[key];
            });

            //================= Conver Object to Array ============================//
            $scope.deptureFlight = Object.keys(objdeptureFlight).map(function (key) {
                return objdeptureFlight[key];
            });

            $scope.deptureFlightNumber = [];
            //================== Rmove Same Flight With Diffrent Amount =============//
            $scope.deptureFlight = $scope.deptureFlight.filter(function (x) {
                let retVal = true;
                let createUniqueFlight = "";
                for (let index = 0; index < x.length; index++) {
                    const element = x[index];
                    createUniqueFlight = createUniqueFlight + element.FlightNo;
                }
                let check = $scope.deptureFlightNumber.indexOf(createUniqueFlight);
                if (check == -1) {
                    $scope.deptureFlightNumber.push(createUniqueFlight);
                } else {
                    return false;
                }
                return retVal;
            });


            $scope.deptureFlightFareList = [];

            //================== Preaper Main Flight List and Fare List ========================//
            for (let i = 0; i < $scope.deptureFlight.length; i++) {

                const element = $scope.deptureFlight[i];
                let result = $scope.getDeptureSameFlightList(element);
                let checkdata = $scope.deptureFlightDetails.filter(function (x) {
                    return x.TrackNo == result[0].TrackNo;
                });
                $scope.deptureFlight[i] = checkdata;


                let fare = $scope.deptureFareDetails.filter(function (item) {
                    return item.SrNo == checkdata[0].SrNo;
                });

                $scope.deptureFlightFareList.push(fare[0])
            }

            //==================== Order By Amount =============================//
            $scope.deptureFlight.sort(function (a, b) {
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
            $scope.selectedDeptFlight = $scope.deptureFlight[0];


            const maxPeak = $scope.deptureFareDetails.reduce((p, c) => Math.round(p.TotalAmount) > Math.round(c.TotalAmount) ? p : c);
            console.log($scope.deptureFareDetails)
            const minPeak = $scope.deptureFareDetails.reduce((p, c) => Math.round(p.TotalAmount) < Math.round(c.TotalAmount) ? p : c);

            $scope.minAmount = Math.round(minPeak.TotalAmount) + $scope.additionalAddedAmount;
            $scope.maxAmount = Math.round(maxPeak.TotalAmount) + $scope.additionalAddedAmount;

            $scope.lower_price_bound = $scope.minAmount;
            $scope.upper_price_bound = $scope.maxAmount;

            $scope.min = $scope.minAmount;
            $scope.max = $scope.maxAmount;

            

        });
    }

    $scope.setDeptureSameFareFlight = function (trackNo, flightNo, index) {
        if (trackNo) {
            let checkdata = $scope.deptureFlightDetails.filter(function (x) {
                return x.TrackNo == trackNo;
            });
            $scope.deptureFlight[index] = checkdata;
            $scope.selectedDeptFlight = checkdata
        }
    };

    $scope.depturegetNetFare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.deptureFareDetails.filter(function (item) {
                return item.SrNo == SrNo;
            });
            if (fare) {
                return parseFloat(fare[0].NetAmount);

            }
            else {
                return 0;
            }
        }
        return 0;
    }

    $scope.getdeptureAirlineName = function (code) {
        let data = $scope.deptureAirlineList.filter(function (x) {
            return x.AirlineCode == code;
        });

        return data[0].AirlineName;
    };

    $scope.getdeptureAirportName = function (code) {
        let data = $scope.deptureAirportList.filter(function (x) {
            return x.AirportCode == code;
        });

        return data[0].AirportName;
    };

    $scope.getDeptureSameFlightList = function (flight) {
        if (flight) {
            let createUniqueFlight = "";
            for (let index = 0; index < flight.length; index++) {
                const element = flight[index];
                createUniqueFlight = createUniqueFlight + element.FlightNo;
            }
            let checkdataTrackNo = $scope.deptureFlightsearchResultTrackNo.filter(function (x) {
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

    $scope.getDeptureFlightTotalBasefare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.deptureFareDetails.filter(function (item) {
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

    $scope.getDeptureOtherFare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.deptureFareDetails.filter(function (item) {
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

    $scope.getdeptureBaggagesInfo = function (SrNo) {
        if (SrNo) {
            let fare = $scope.deptureFareDetails.filter(function (item) {
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

    $scope.getDeptureNetFare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.deptureFareDetails.filter(function (item) {
                return item.SrNo == SrNo;
            });
            if (fare) {
                return parseFloat(fare[0].NetAmount);

            }
            else {
                return 0;
            }
        }
        return 0;
    }

    // #endregion

    // #region Load return flight 
    $scope.loadReturnFlightSearch = function (Tripmode, FromCityCode, TOAirportCode, FromDate, ToDate, TravelType, Adult, Child, Infant) {


        const data = {
            Tripmode: '1',
            FromAirportsName: TOAirportCode,
            FromCityCode: TOAirportCode,
            TOAirportName: FromCityCode,
            TOAirportCode: FromCityCode,
            FromDate: ToDate,
            ToDate: FromDate,
            TravelType: TravelType,
            Adult: Adult,
            Child: Child,
            Infant: Infant
        };


        const service = FlightServices.getFlightSingleSearchDetails(data);
        service.then(function (response) {

            const data = response.data;
            const FlightResponse = JSON.parse(data);

            //=========================== Store All Flight Information =============================//
            $scope.returnFlightDetails = FlightResponse.GetFlightAvailibilityResponse.FlightDetails;
            $scope.returnFareDetails = FlightResponse.GetFlightAvailibilityResponse.FareDetails;
            $scope.returnAirlineList = FlightResponse.GetFlightAvailibilityResponse.AirlineList;
            $scope.returnAirportList = FlightResponse.GetFlightAvailibilityResponse.AirportList;


            $scope.inBoundAirlinesList = $scope.returnAirlineList.map(item => {
                const container = {};
                container.name = item.AirlineName;
                container.code = item.AirlineCode;
                container.selected = false;
                return container;
            });


            //================= Group by track number =============================//
            let objreturnFlight = $filter('groupBy')($scope.returnFlightDetails, 'TrackNo');

            //================= Store a copy for feature uses =======================//
            $scope.returnFlightsearchResultTrackNo = Object.keys(objreturnFlight).map(function (key) {
                return objreturnFlight[key];
            });

            //================= Conver Object to Array ============================//
            $scope.returnFlight = Object.keys(objreturnFlight).map(function (key) {
                return objreturnFlight[key];
            });

            $scope.returnFlightNumber = [];
            //================== Rmove Same Flight With Diffrent Amount =============//
            $scope.returnFlight = $scope.returnFlight.filter(function (x) {
                let retVal = true;
                let createUniqueFlight = "";
                for (let index = 0; index < x.length; index++) {
                    const element = x[index];
                    createUniqueFlight = createUniqueFlight + element.FlightNo;
                }
                let check = $scope.returnFlightNumber.indexOf(createUniqueFlight);
                if (check == -1) {
                    $scope.returnFlightNumber.push(createUniqueFlight);
                } else {
                    return false;
                }
                return retVal;
            });


            $scope.returnFlightFareList = [];

            //================== Preaper Main Flight List and Fare List ========================//
            for (let i = 0; i < $scope.returnFlight.length; i++) {

                const element = $scope.returnFlight[i];
                let result = $scope.getreturnSameFlightList(element);
                let checkdata = $scope.returnFlightDetails.filter(function (x) {
                    return x.TrackNo == result[0].TrackNo;
                });
                $scope.returnFlight[i] = checkdata;


                let fare = $scope.returnFareDetails.filter(function (item) {
                    return item.SrNo == checkdata[0].SrNo;
                });

                $scope.returnFlightFareList.push(fare[0])
            }

            //==================== Order By Amount =============================//
            $scope.returnFlight.sort(function (a, b) {
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

            debugger;
       

            $scope.selectedReturnFlight = $scope.returnFlight[0];

            const ReturnmaxPeak = $scope.returnFareDetails.reduce((p, c) => Math.round(p.TotalAmount) > Math.round(c.TotalAmount) ? p : c);
            const ReturnminPeak = $scope.returnFareDetails.reduce((p, c) => Math.round(p.TotalAmount) < Math.round(c.TotalAmount) ? p : c);


            $scope.ReturnminAmount = Math.round(ReturnminPeak.TotalAmount) + $scope.additionalAddedAmount;
            $scope.ReturnmaxAmount = Math.round(ReturnmaxPeak.TotalAmount) + $scope.additionalAddedAmount;

            $scope.Return_lower_price_bound = $scope.ReturnminAmount;
            $scope.Return_upper_price_bound = $scope.ReturnmaxAmount;

            $scope.Return_min = $scope.ReturnminAmount;
            $scope.Return_max = $scope.ReturnmaxAmount;
        });
    }

    $scope.setreturnSameFareFlight = function (trackNo, flightNo, index) {
        if (trackNo) {
            let checkdata = $scope.returnFlightDetails.filter(function (x) {
                return x.TrackNo == trackNo;
            });
            $scope.returnFlight[index] = checkdata;
            $scope.selectedReturnFlight = checkdata;
        }
    };

    $scope.returngetNetFare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.returnFareDetails.filter(function (item) {
                return item.SrNo == SrNo;
            });
            if (fare) {
                return parseFloat(fare[0].NetAmount);

            }
            else {
                return 0;
            }
        }
        return 0;
    }

    $scope.getreturnAirlineName = function (code) {
        let data = $scope.returnAirlineList.filter(function (x) {
            return x.AirlineCode == code;
        });

        return data[0].AirlineName;
    };

    $scope.getreturnAirportName = function (code) {
        let data = $scope.returnAirportList.filter(function (x) {
            return x.AirportCode == code;
        });

        return data[0].AirportName;
    };

    $scope.getreturnSameFlightList = function (flight) {
        if (flight) {
            let createUniqueFlight = "";
            for (let index = 0; index < flight.length; index++) {
                const element = flight[index];
                createUniqueFlight = createUniqueFlight + element.FlightNo;
            }
            let checkdataTrackNo = $scope.returnFlightsearchResultTrackNo.filter(function (x) {
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

    $scope.getreturnFlightTotalBasefare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.returnFareDetails.filter(function (item) {
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

    $scope.getreturnOtherFare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.returnFareDetails.filter(function (item) {
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

    $scope.getreturnBaggagesInfo = function (SrNo) {
        if (SrNo) {
            let fare = $scope.returnFareDetails.filter(function (item) {
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

    $scope.getReturnNetFare = function (SrNo) {
        if (SrNo) {
            let fare = $scope.returnFareDetails.filter(function (item) {
                return item.SrNo == SrNo;
            });
            if (fare) {
                return parseFloat(fare[0].NetAmount);

            }
            else {
                return 0;
            }
        }
        return 0;
    }

    // #endregion



    //================= Common =====================//
  

    $scope.getFormatDate = function (date) {
        let splitDate = date.split("/");
        return moment(`${splitDate[1]}/${splitDate[0]}/${splitDate[2]}`).format(
            "ddd D MMM YY"
        );
    };

    function diff_minutes(dt2, dt1) {

        var diff = (dt2.getTime() - dt1.getTime()) / 1000;
        diff /= 60;
        return Math.abs(Math.round(diff));

    }

    $scope.timeConvert = function (n) {
        var num = n;
        var hours = (num / 60);
        var rhours = Math.floor(hours);
        var minutes = (hours - rhours) * 60;
        var rminutes = Math.round(minutes);
        return rhours + "h " + rminutes + "m";
    }

    $scope.calculateDuration = function (item) {
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


    function nl2br(str, is_xhtml) {
        var breakTag = (is_xhtml || typeof is_xhtml === 'undefined') ? '<br />' : '<br>';
        return (str + '').replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, '$1' + breakTag + '$2');
    }

    $scope.showFareRules = function (trackNo) {
        const data = { TrackNo: trackNo };
        const service = FlightServices.getFareRules(data);
        service.then(function (response) {
            const responce = response.data;
            const fareRules = JSON.parse(responce);

            $scope.fareRules = fareRules.GetFareRuleResponse.Farerules[0];
            $('#pFareRules').html('');
            $('#pFareRules').html(nl2br($scope.fareRules))

            $('#modalFareRules').modal('show');

        });
    }

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
        if (item.length > 0) {
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

    $scope.totalAmountCalculation = function (amount) {
        debugger;
        if (amount) {
            return parseFloat(amount) + $scope.additionalAddedAmount;
        }
        return 0;
    }

    $scope.bookingTotalAmount = function (DerpAmt, RetAmt) {
        debugger;
        if (DerpAmt.length > 0 && RetAmt.length > 0) {
            let TotalAmt = 0;
            if (DerpAmt[0].TotalAmount && RetAmt[0].TotalAmount != undefined) {
                //TotalAmt = parseFloat(DerpAmt[0].TotalAmount) + parseFloat(RetAmt[0].TotalAmount) + ($scope.additionalAddedAmount == 0 ? 0 : ($scope.additionalAddedAmount * 2));
                TotalAmt = parseFloat(DerpAmt[0].TotalAmount) + parseFloat(RetAmt[0].TotalAmount);
            }
            else {
                TotalAmt = 0;
            }
            return Math.round(TotalAmt);
        }
    }

    $scope.roundtripgetFlightDetails = function (Adult, Children, Infant, TrackNo, TripMode, OriginDeptCode, DestinationReturnCode) {
        debugger;
        //window.location.href = '/Merchant/MerchantFlightDetails/FlightBookingDetails?TrackNo=' + TrackNo + '&PsgnAdult=' + Adult + '&PsgnChildren=' + Children + '&PsgnInfant=' + Infant + '&TripMode=' + TripMode;
        window.location.href = '/Merchant/MerchantFlightDetails/FlightBookingDetails?TrackNo=' + TrackNo + '&PsgnAdult=' + Adult + '&PsgnChildren=' + Children + '&PsgnInfant=' + Infant + '&TripMode=' + TripMode + '&OriginCode=' + OriginDeptCode + '&DestinationCode=' + DestinationReturnCode;
    };


}]);