var app = angular.module('AirportAutocompleteoduleApp', ['ngAnimate','angucomplete-alt', 'angular-loading-bar', 'angular.filter', 'checklist-model', 'infinite-scroll', 'uiSlider', 'ui.bootstrap']).config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
    cfpLoadingBarProvider.includeBar = true;
}]);