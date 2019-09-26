var app = angular.module('app').controller('AUAASACtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {

    $scope.sortType = 'TotalCount';
    $scope.sortReverse = false;

    $scope.Option = [
    { id: 1, Time: 'Last 1 Hour', value: '1' },
    { id: 2, Time: 'Today', value: 'Today' },
];

    $(window).load(function () {
        $scope.changedValue(2);
    });

    $scope.changedValue = function (item) {
        if (item == 2 || item == null) {
            var DatTime = 2;
        }
        else {
            var DatTime = item.value;
        }

        $scope.param = null;
        var item = {};
        item.DateTime = DatTime;
        item.RequestType = 'Main';
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessAUAASA";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.AUAASAGrid == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response = JSON.parse(data.AUAASAGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };

    $scope.GetDetail = function (Instance_Id, Authres_err, First_txn_time) {
        $scope.param = null;
        var item = {};        
        item.RequestType = 'GetDetail';
        item.Instance_Id = Instance_Id;
        item.Authres_err = Authres_err;
        item.Log_Date = First_txn_time;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessAUAASA";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.AUAASAGrid == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response1 = JSON.parse(data.AUAASAGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    }

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'TotalCount';
        $scope.sortReverse = !ascdesc;
    }

    //    window.setInterval(function () {
    //        location.reload();
    //    }, 120000);
} ]);
