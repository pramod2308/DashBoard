var app = angular.module('app').controller('AEPSDashBoardCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {

    $scope.sortType = 'RESPCODE';
    $scope.sortReverse = false;

    $scope.Option = [
    { id: 1, Time: 'Last 1 Hour', value: '1' },
    { id: 2, Time: 'Today', value: 'Today' },
     ];

    $(window).load(function () {
        $scope.changedValue(2);
        $scope.GetAEPSFailedData();
    });

    var dt;
    $scope.changedValue = function (item) {
        var DatTime = 0;
        if (item == 2 || item == null) {
            DatTime = 2;
        }
        else {
            DatTime = item.value;
        }
        dt = DatTime;

        $scope.GetAEPSFailedData();

    };

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'RESPCODE';
        $scope.sortReverse = !ascdesc;
    }

    window.setInterval(function () {
        location.reload();
    }, 120000);

    $scope.GetAEPSFailedData = function () {

        $scope.param = null;
        var item = {};
        item.DateTime = dt;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessAEPSFailed";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.AEPSFailedResponse = JSON.parse(data.AEPSGrid);

                if ($scope.AEPSFailedResponse.length == 0) {
                     alert('Records Not Found');
                }
            }
        });

    }
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