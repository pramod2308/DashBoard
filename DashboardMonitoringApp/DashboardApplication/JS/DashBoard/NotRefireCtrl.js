var app = angular.module('app').controller('NotRefireCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'Logdatetime';
    $scope.sortReverse = true;

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

        var item = {};
        item.refiredatetime = DatTime;
        item.refire = 2;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/Processrefire";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.Response = JSON.parse(data.dtrefireresponse);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });

    };

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'Logdatetime';
        $scope.sortReverse = !ascdesc;
    }

    //    window.setInterval(function () {
    //        location.reload();
    //    }, 120000);

} ]);
