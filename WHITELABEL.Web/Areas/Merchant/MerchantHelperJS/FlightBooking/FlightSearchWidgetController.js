app.controller('FlightSearchWidgetController', ['FlightServices', '$scope', '$http', '$window', '$timeout', function (FlightServices, $scope, $http, $window, $timeout) {
    $scope.travellType = 'Y';
    $scope.adultsCount = 1;
    $scope.childsCount = 0;
    $scope.infantsCount = 0;
    $scope.tripType = '1';
    $scope.getAirport = function (val) {        
        return $http.get('/Merchant/MerchantFlightBooking/GetAllAirports', {
            params: {
                req: val,
            }
        }).then(function (response) {
            const msgval = response.data;
            if (msgval == '1') {
                location.reload();
                const url = "/Login/Index";
                window.location.href = url;
                return response.data;
            }
            else { return response.data; }
            //return response.data;
        });
    };
    $scope.InterchangeLocation = function (Dept, Return) {     
        const DeptureVal = Dept;
        const ReturnVal = Return;
        $scope.fromAirportDetails = Return;
        $scope.toAirportDetails = Dept;
    }
    $scope.newDt = new Date();

    $scope.dynamicPopover = {
        content: '',
        templateUrl: 'myPopoverTemplate.html',
        title: ''
    };


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

   

   

    $scope.passengerCount = 1;
    $scope.updatePassenger = function (adultsCount, childsCount, infantsCount) {

        $scope.adultsCount = adultsCount;
        $scope.childsCount = childsCount;
        $scope.infantsCount = infantsCount;
        debugger;
        if ($scope.infantsCount > $scope.adultsCount) {

            $scope.infantsCount = $scope.adultsCount;
        }
        $scope.passengerCount = $scope.adultsCount + $scope.childsCount + $scope.infantsCount;

    }
    $scope.travellTypeText = 'Economy';
    
    $scope.updateTravellType = function (travellType) {
        $scope.travellType = travellType;

        if (travellType == 'Y') {
            $scope.travellTypeText = 'Economy';
        }
        else if (travellType == 'C') {
            $scope.travellTypeText = 'Business'
        }
        else if (travellType == 'W') {
            $scope.travellTypeText = 'Premium Economy'
        }
        else if (travellType == 'F') {
            $scope.travellTypeText = 'First Class'
        }
    };

    $scope.submitSearch = function () {
        if (parseInt($scope.tripType) == 1) {
            var url = `/Merchant/MerchantFlightDetails/GetSearchResult?Tripmode=${parseInt($scope.tripType)}&FromCityCode=${$scope.fromAirportDetails.CITYCODE}&TOAirportCode=${$scope.toAirportDetails.CITYCODE}&FromDate=${moment($scope.startDate).format('YYYY-MM-DD')}&ToDate=&TravelType=${$scope.travellType}&Adult=${$scope.adultsCount}&Child=${$scope.childsCount}&Infant=${$scope.infantsCount}`;
            window.location.href = url;
        }
        else if (parseInt($scope.tripType) == 2) {
            var url = `/Merchant/MerchantFlightDetails/ReturnFlightlist?Tripmode=${parseInt($scope.tripType)}&FromCityCode=${$scope.fromAirportDetails.CITYCODE}&TOAirportCode=${$scope.toAirportDetails.CITYCODE}&FromDate=${moment($scope.startDate).format('YYYY-MM-DD')}&ToDate=${moment($scope.endDate).format('YYYY-MM-DD')}&TravelType=${$scope.travellType}&Adult=${$scope.adultsCount}&Child=${$scope.childsCount}&Infant=${$scope.infantsCount}`;
            window.location.href = url;
        }
    }

    $scope.initSetSearchValue = function () {

        $timeout(function () {
            
            const data = localStorage.getItem('SEARCH_FLIGHT_DATA');
            if (data) {
                const searchObj = JSON.parse(data);

                $scope.travellType = searchObj.TravelType;
                $scope.updateTravellType($scope.travellType);
                $scope.adultsCount = parseInt(searchObj.Adult ? searchObj.Adult : 0);
                $scope.childsCount = parseInt(searchObj.Child ? searchObj.Child : 0);
                $scope.infantsCount = parseInt(searchObj.Infant ? searchObj.Infant : 0);
                $scope.tripType = searchObj.Tripmode;
                $scope.updatePassenger($scope.adultsCount, $scope.childsCount, $scope.infantsCount);
                $scope.startDate = new Date(searchObj.FromDate);
                if (parseInt($scope.tripType) == 2) {
                    $scope.endDate = new Date(searchObj.ToDate);
                }
                
              
                var getFormAirport = $http.get('/Merchant/MerchantFlightBooking/GetAllAirportsByCitycode', {
                    params: {
                        code: searchObj.FromCityCode,
                    }
                }).then(function (response) {
                    $scope.fromAirportDetails = response.data;
                });


                var getToAirport = $http.get('/Merchant/MerchantFlightBooking/GetAllAirportsByCitycode', {
                    params: {
                        code: searchObj.TOAirportName,
                    }
                }).then(function (response) {
                    $scope.toAirportDetails = response.data;
                });
            }
        }, 300);
       
    }

    $scope.initSetSearchValue();

}]);

