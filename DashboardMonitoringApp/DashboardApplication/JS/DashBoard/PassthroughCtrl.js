var app = angular.module('app').controller('PassthroughCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {

    $scope.sortType = 'Request_Type';
    $scope.sortReverse = false;

    $scope.Option = [
    { id: 1, Time: 'Last 1 Hour', value: '1' },
    { id: 2, Time: 'Today', value: 'Today' },
];

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
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessPassthrough";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.PassthroughGrid == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response = JSON.parse(data.PassthroughGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };
    $scope.changedValue();

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'Local_Date_Time';
        $scope.sortReverse = !ascdesc;
    }

    //    window.setInterval(function () {
    //        location.reload();
    //    }, 120000);
} ]);
