var app = angular.module('app').controller('SecondDashBoardCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'Request_Type';
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
        item.DashboardType = 2;
        item.DateTime = DatTime;
        $scope.param = item;
        $scope.URL = "/DashboardAppService.svc/ProcessDashboardRequest";
        $('#loader').fadeIn();
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {            
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.Response = JSON.parse(data.DashboardGrid);
                
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

    window.setInterval(function () {
        location.reload();
    }, 120000);

} ]);
