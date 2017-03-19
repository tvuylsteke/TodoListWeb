'use strict';
angular.module('todoApp', ['ngRoute','AdalAngular'])
    //http://slopjong.de/2015/09/01/set-global-constants-and-variables-in-angularjs/
    .constant('config', {
        apiURL: "https://todolistapi.setspn.be.eu.org"
    })

    .config(['$routeProvider', '$httpProvider', 'adalAuthenticationServiceProvider', function ($routeProvider, $httpProvider, adalProvider) {

    $routeProvider.when("/Home", {
        controller: "homeCtrl",
        templateUrl: "/App/Views/Home.html",
    }).when("/TodoList", {
        controller: "todoListCtrl",
        templateUrl: "/App/Views/TodoList.html",
        requireADLogin: true,
    }).when("/UserData", {
        controller: "userDataCtrl",
        templateUrl: "/App/Views/UserData.html",
    }).otherwise({ redirectTo: "/Home" });
    
    Logging = {
        level: 3,
        log: function (message) {
            console.log(message);
        }
    };

    adalProvider.init(
        {
            instance: 'https://sts.setspn.be.eu.org/', 
            tenant: 'adfs',
            clientId: '9e027f2c-41f2-4464-9774-bdad7bfdf653',
            extraQueryParameter: 'nux=1',
            //cacheLocation: 'localStorage', // enable this for IE, as sessionStorage does not work for localhost.
            endpoints: { "https://todolistapi.setspn.be.eu.org": "https://todolistapi.setspn.be.eu.org" }
        },
        $httpProvider
        );
   
}]);
