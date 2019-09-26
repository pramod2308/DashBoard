angular.module('app', []);

//var app = angular.module('app', ['ui.router']);

//app.config(function ($stateProvider, $urlRouterProvider) {

//    $urlRouterProvider.otherwise('/Response Time');

//    $stateProvider
//        
//        .state('First', {
//            url: '/RequestIN-OUT',
//            templateUrl: 'HtmlPages/DashBoard/FirstDashBoard.html',
//            controller:'FirstDashBoardCtrl'
//        })

//       .state('Second', {
//           url: '/Response Time',
//           templateUrl: 'HtmlPages/DashBoard/SecondDashBoard.html',
//           controller: 'SecondDashBoardCtrl'
//       })

//       .state('Third', {
//           url: '/Network Exception',
//           templateUrl: 'HtmlPages/DashBoard/Status_Code_Count.html'
//       })

//       .state('Fourth', {
//           url: '/Failure Details',
//           templateUrl: 'HtmlPages/DashBoard/FourtDashBoard.html'
//       })

//       .state('Fifth', {
//           url: '/Transactions',
//           templateUrl: 'HtmlPages/DashBoard/FifthDashBoard.htm'
//       })

//       .state('Sixth', {
//           url: '/Failed-Awaiting',
//           templateUrl: 'HtmlPages/DashBoard/ESBFailed_Awaiting.htm'
//       })

//       .state('Seventh', {
//           url: '/Service Latency',
//           templateUrl: 'HtmlPages/DashBoard/Service_Latency.htm'
//       })

//       .state('Eighth', {
//           url: '/Node Latency',
//           templateUrl: 'HtmlPages/DashBoard/Node_Latency.htm'
//       })

//       .state('Ninth', {
//           url: '/Merchant Transaction',
//           templateUrl: 'HtmlPages/DashBoard/MerchantTransaction.htm'
//       })

//   });

//    app.factory('ServiceCall', function ($http, $q) {
//       return {

//           PostData: function (URL, param) {
//               var deferred = $q.defer();
//               $http({
//                   url: URL,
//                   method: "POST",
//                   data: param,
//                   contentType: "application/json; charset=utf-8",
//                   dataType: "json"

//               }).success(function (data) {
//                   deferred.resolve(data);
//               }).error(function (data) {
//                   deferred.reject();
//               });

//               return deferred.promise;

//           }

//       }

//   });

//   app.factory('cache', function ($cacheFactory) {
//       var cache = $cacheFactory('PBcache');
//       return cache;
//   });
