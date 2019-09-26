var app = angular.module('app').controller('CashBazaarCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {


    $scope.GetData = function () {
        $scope.changedValue();
        $scope.changedValue2();
    }
    $scope.changedValue = function (item) {

        $scope.param = null;
        var item = {};
        item.COLAID = '1';
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/CashBazaarResponse";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.dtsuryodayresp == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response = JSON.parse(data.CashBazaarGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };

    $scope.changedValue2 = function (item) {

        $scope.param = null;
        var item = {};
        item.COLAID = '2';
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/CashBazaarResponse";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.dtsuryodayresp == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response1 = JSON.parse(data.CashBazaarGrid);

                if ($scope.Response1.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };
    $scope.changedValue();
    $scope.changedValue2();

    //    window.setInterval(function () {
    //        location.reload();
    //    }, 120000);
} ]);
