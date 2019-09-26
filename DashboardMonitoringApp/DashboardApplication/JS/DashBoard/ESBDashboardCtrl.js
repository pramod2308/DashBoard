var app = angular.module('app').controller('ctrlESB', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'AVGS';
    $scope.sortReverse = true;
    $scope.requetype = 'FindCustomer';

    $scope.Option = [
    { id: 1, Time: 'Last 1 Hour', value: '1' },
    { id: 2, Time: 'Today', value: 'Today' },
];

    $scope.GetRequestType = function () {

        var item = {};
        item.Request_Type = 'Nothing';
        $scope.param = item;
      //  $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessToGetRequestType";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
             //   $('#loader').fadeOut('slow');
                $scope.RequestTypeResponse = JSON.parse(data.ServiceAndMethodID);

                if ($scope.RequestTypeResponse.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.GetRequestType();

    $scope.MethodId = "";
    $scope.ServiceId = "";

    $scope.GetServiceAndMethodID = function (reqtype) {

        $scope.requetype = reqtype.Request_Type;

        var item = {};
        item.Request_Type = reqtype.Request_Type;
        $scope.param = item;
       // $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessToGetRequestType";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
             //   $('#loader').fadeOut('slow');
                $scope.RequestTypeMetandserviveIDResponse = JSON.parse(data.ServiceAndMethodID);

                if ($scope.RequestTypeMetandserviveIDResponse.length == 0) {
                    alert('Records Not Found');
                }

                $scope.MethodId = $scope.RequestTypeMetandserviveIDResponse[0].Method_ID;
                $scope.ServiceId = $scope.RequestTypeMetandserviveIDResponse[0].Service_ID;
                $scope.changedValue(DatTime);
            }
        });

    };


    $(window).load(function () {
        $scope.changedValue(2);
    });

    var DatTime = '';
    $scope.changedValue = function (item) {
        var MID = '';
        var SID = '';

        if (item == 2 || item == null) {
            DatTime = 2;
        }
        else {
            DatTime = item.value;
        }

        if ($scope.MethodId == "" && $scope.ServiceId == "") {
            MID = 8;
            SID = 2;
        }
        else {
            MID = $scope.MethodId;
            SID = $scope.ServiceId;
        }

        var item = {};
        item.DateTime = DatTime;
        item.LatencyType = 'NoLayer';
        item.MetID = MID;
        item.SerID = SID;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessESB";
       // $scope.URL = "/DashboardAppService.svc/ProcessESB";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.Response = JSON.parse(data.ZoneGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'AVGS';
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
