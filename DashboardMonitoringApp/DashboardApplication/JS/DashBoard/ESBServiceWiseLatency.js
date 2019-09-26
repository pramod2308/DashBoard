var app = angular.module('app').controller('ctrlESBServiceLatency', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'TotalHits';
    $scope.sortReverse = true;

    $scope.Option = [
    { id: 1, Time: 'Last 10 Minute', value: '10' },
    { id: 2, Time: 'Last 15 Minute', value: '15' },
    { id: 3, Time: 'Last 30 Minute', value: '30' },
    { id: 4, Time: 'Last 1 Hour', value: '1' },
    { id: 5, Time: 'Today', value: 'Today' }
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
        item.LatencyType = 'Service';
        item.DateTime = DatTime;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/PBDashboardApp/DashboardAppService.svc/ProcessESBNODESERVICELatency";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.Response = JSON.parse(data.LatencyGrid);

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

    window.setInterval(function () {
        location.reload();
    }, 120000);

} ]);
