var app = angular.module('app').controller('IMPSCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {

    $scope.sortType = 'DateTime';
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
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessIMPSData";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                
                $scope.Response = JSON.parse(data.IMPSResponseData);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'DateTime';
        $scope.sortReverse = !ascdesc;
    }

    window.setInterval(function () {
        location.reload();
    }, 900000);
} ]);
