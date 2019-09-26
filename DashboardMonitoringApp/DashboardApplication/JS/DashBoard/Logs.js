
var app = angular.module('app').controller('LogCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.CorTable = false;
    $scope.Logs = false;
    

    $scope.Logss = function () {
        debugger;
        $scope.Logs = true;
        $scope.CorTable = false;
        var item = {};
        item.RequestId = $scope.RequestId;
        item.tblName = "TblLog";
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/PBDashboardApp/DashboardAppService.svc/ProcessGetLogs";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.LogResponse = JSON.parse(data.LogResponse);

                if ($scope.LogResponse.length == 0) {
                    alert('Records Not Found');
                }
            }
        });
    }


    $scope.Corelation = function () {
        debugger;
        $scope.Logs = false;
        $scope.CorTable = true;
        var item = {};
        item.RequestId = $scope.RequestId;
        item.tblName = "Tbl_Corelation_Log";
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/PBDashboardApp/DashboardAppService.svc/ProcessGetLogs";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.CorelationResponse = JSON.parse(data.LogResponse);

                if ($scope.CorelationResponse.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    }

} ]);


