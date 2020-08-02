var app = angular.module('AirportAutocompleteoduleApp', ['angucomplete-alt', 'angular-loading-bar', 'angular.filter', 'checklist-model', 'infinite-scroll', 'ui.bootstrap']).config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;
    cfpLoadingBarProvider.includeBar = true;
}]);