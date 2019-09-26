﻿var app = angular.module('app').controller('CMSAPICtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'Request_Type';
    $scope.sortReverse = false;

    $scope.Option = [
    { id: 1, Time: 'Last 1 Hour', value: '1' },
    { id: 2, Time: 'Today', value: 'Today' },
];
    $scope.DateTime = "";
    $scope.EndDateTime = "";    

    debugger;
    var d = new Date();
    var Year = d.getFullYear();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var hour = d.getHours();
    var minute = d.getMinutes();
    var second = d.getSeconds();

    //2017/12/7 13:26
    $scope.DateTime = Year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
    $scope.EndDateTime = Year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;

    $(window).load(function () {
        $scope.changedValue(2);
    });

    $scope.changedValue = function (item) {
        if (item == 2 || item == null) {
            minute = d.getMinutes() - 2;
            hour = d.getHours();
            $scope.DateTime = Year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
        }
        else if (item.value == 1) {
            minute = d.getMinutes();
            hour = d.getHours() - 1;
            $scope.DateTime = Year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
        }
        else {
            $scope.DateTime = Year + "/" + month + "/" + 8;
            $scope.EndDateTime = Year + "/" + month + "/" + day;
        }

        $scope.param = null;
        var item = {};
        item.CMSStatus = 'GetAllData';
        item.DateTime = $scope.DateTime;
        item.EndDateTime = $scope.EndDateTime;
        $scope.param = item;
        $scope.URL = "/DashboardAppService.svc/CMSAPIResponse";
        $('#loader').fadeIn();
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                debugger;
                $scope.Response = JSON.parse(data.CMSDashboardGrid);
                for (var i = 0; i < $scope.Response.length; i++) {
                    $scope.Initiated = $scope.Initiated + $scope.Response[i]["Initiated"];
                    $scope.Success = $scope.Success + $scope.Response[i]["Success"];
                    $scope.Failed = $scope.Failed + $scope.Response[i]["Failed"];
                }

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.GetSuccessData = function (PartnerID, productcode) {
        debugger;        
        $('.collapse').addClass('collapse').removeClass('in');

        $scope.param = null;
        var item = {};
        item.PartnerID = PartnerID;
        item.productcode = productcode;
        item.CMSStatus = 'GetSuccessData';
        item.DateTime = $scope.DateTime;
        item.EndDateTime = $scope.EndDateTime;
        $scope.param = item;
        $scope.URL = "/DashboardAppService.svc/CMSAPIResponse";
        $('#loader').fadeIn();
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.SuccessResponse = JSON.parse(data.CMSDashboardGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.GetFailedData = function (PartnerID, productcode) {
        debugger;        
        $('.collapse').addClass('collapse').removeClass('in');

        $scope.param = null;
        var item = {};
        item.PartnerID = PartnerID;
        item.productcode = productcode;
        item.CMSStatus = 'GetFailedData';
        item.DateTime = $scope.DateTime;
        item.EndDateTime = $scope.EndDateTime;
        $scope.param = item;
        $scope.URL = "/DashboardAppService.svc/CMSAPIResponse";
        $('#loader').fadeIn();
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.FailedResponse = JSON.parse(data.CMSDashboardGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }
            }
        });
    };

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'Request_Type';
        $scope.sortReverse = !ascdesc;
    }

    //    window.setInterval(function () {
    //        location.reload();
    //    }, 120000);

} ]);

angular.module('app').filter('sumOfValue', function () {
    return function (data, key) {
        if (typeof (data) === 'undefined' || typeof (key) === 'undefined') {
            return 0;
        }
        var sum = 0;
        for (var i = 0; i < data.length; i++) {
            sum = sum + parseInt(data[i][key]);
        }
        return sum;
    }
});