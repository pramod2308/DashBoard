var app = angular.module('app').controller('EKOServerStatusCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'Req_Count';
    $scope.sortReverse = true;

    $scope.Option = [
    { id: 1, Time: 'Last 1 Hour', value: '1' },
    { id: 2, Time: 'Today', value: 'Today' },
];

    $scope.EndDateTime = "";

    var d = new Date();
    var Year = d.getFullYear();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var hour = d.getHours();
    var minute = d.getMinutes();
    var second = d.getSeconds();

    var DateTime = Year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
    $scope.EndDateTime = Year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;

    $(window).load(function () {
        $scope.changedValue(2);
    });

    $scope.changedValue = function (item) {
        if (item == 2 || item == null) {
            minute = d.getMinutes() - 2;
            hour = d.getHours();
            DateTime = Year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
        }
        else if (item.value == 1) {
            minute = d.getMinutes();
            hour = d.getHours() - 1;
            DateTime = Year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
        }
        else {
            DateTime = Year + "/" + month + "/" + day;
        }

        $scope.param = null;
        var item = {};
        item.EKOStatus = 'ServerStatus';
        item.DateTime = DateTime;
        item.EndDateTime = $scope.EndDateTime;
        $scope.param = item;
        $scope.URL = "/DashboardAppService.svc/ProcessToGetEKO";
        $('#loader').fadeIn();
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.Response = JSON.parse(data.EKODashboardGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'Req_Count';
        $scope.sortReverse = !ascdesc;
    }

//        window.setInterval(function () {
//            location.reload();
//        }, 120000);

} ]);
