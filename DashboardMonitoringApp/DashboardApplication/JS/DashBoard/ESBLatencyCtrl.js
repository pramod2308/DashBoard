var app = angular.module('app').controller('ctrlESBlatency', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.Option = [
    { id: 1, Time: 'Last 1 Hour', value: '1' },
    { id: 2, Time: 'Today', value: 'Today' },
];
    $scope.requetype = 'FindCustomer';
    $scope.bpayrequetype = 'MBCustomerLogin';

    $scope.GetRequestType = function () {

        var item = {};
        item.Request_Type = 'Nothing';
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessToGetRequestType";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.RequestTypeResponse = JSON.parse(data.ServiceAndMethodID);

                if ($scope.RequestTypeResponse.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.GetBPayRequestType = function () {

        var item = {};
        item.Request_Type = 'BPay';
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessToGetRequestType";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.BPayRequestTypeResponse = JSON.parse(data.ServiceAndMethodID);

                if ($scope.BPayRequestTypeResponse.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.GetRequestType();
    $scope.GetBPayRequestType();

    $scope.MethodId = "";
    $scope.ServiceId = "";

    $scope.GetServiceAndMethodID = function (reqtype) {

        $scope.requetype = reqtype.Request_Type;

        var item = {};
        item.Request_Type = reqtype.Request_Type;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessToGetRequestType";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
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

    $scope.BPayMethodId = "";
    $scope.BPayServiceId = "";
    $scope.GetBPayServiceAndMethodID = function (bpayreqtype) {

        $scope.bpayrequetype = bpayreqtype.Request_Type;

        var item = {};
        item.Request_Type = bpayreqtype.Request_Type;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessToGetRequestType";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.RequestTypeMetandserviveIDResponse = JSON.parse(data.ServiceAndMethodID);

                if ($scope.RequestTypeMetandserviveIDResponse.length == 0) {
                    alert('Records Not Found');
                }

                $scope.BPayMethodId = $scope.RequestTypeMetandserviveIDResponse[0].Method_ID;
                $scope.BPayServiceId = $scope.RequestTypeMetandserviveIDResponse[0].Service_ID;
                $scope.BPaychangedValue(DatTime);
            }
        });

    };

    $(window).load(function () {
        $scope.changedValue(2);
    });


    var DatTime = '';
    $scope.changedValue = function (item) {

        var SID = '';

        if (item == 2 || item == null) {
            DatTime = 2;
        }
        else {
            DatTime = item.value;
        }

        $scope.BPaychangedValue(DatTime);
        var MID = '';

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
        item.LatencyType = 'Layer';
        item.MetID = MID;
        item.SerID = SID;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessESB";
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

    $scope.BPaychangedValue = function (bpayitem) {
        var BPayMID = '';
        var BPaySID = '';

        if ($scope.BPayMethodId == "" && $scope.BPayServiceId == "") {
            BPayMID = 1102;
            BPaySID = 101;
        }
        else {
            BPayMID = $scope.BPayMethodId;
            BPaySID = $scope.BPayServiceId;
        }

        var item = {};
        item.DateTime = bpayitem;
        item.LatencyType = 'BPay';
        item.MetID = BPayMID;
        item.SerID = BPaySID;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessESB";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.BPayResponse = JSON.parse(data.ZoneGrid);
            }
        });

    };

    window.setInterval(function () {
        location.reload();
    }, 120000);

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
