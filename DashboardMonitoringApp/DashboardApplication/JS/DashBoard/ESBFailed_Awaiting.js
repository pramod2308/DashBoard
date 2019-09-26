var app = angular.module('app').controller('SixthDashBoardCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {

    $scope.sortType = 'RESPCODE';
    $scope.sortReverse = false;

    $scope.Option = [
    { id: 1, Time: 'Last 1 Hour', value: '1' },
    { id: 2, Time: 'Today', value: 'Today' },
     ];

    $(window).load(function () {
        $scope.changedValue(2);
        $scope.GetIMPSFailedData();
        $scope.GetIMPSAwaitingData();
        $scope.GetBeneFailedData();
        $scope.GetBeneAwaitingData();
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

        $scope.GetIMPSFailedData();
        $scope.GetIMPSAwaitingData();
        $scope.GetBeneFailedData();
        $scope.GetBeneAwaitingData();

    };   

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'RESPCODE';
        $scope.sortReverse = !ascdesc;
    }

    window.setInterval(function () {
        location.reload();
    }, 120000);

    $scope.GetIMPSFailedData = function () {

        $scope.param = null;
        var item = {};
        item.ESBTab = 1;
        item.TType = 'Failed';
        item.TransactionName = 'IMPS';
        item.DateTime = dt;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessFailedAwaiting";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.IMPSFailedResponse = JSON.parse(data.ESBGrid);
               
                if ($scope.IMPSFailedResponse.length == 0) {
                    // alert('Records Not Found');
                }
            }
        });
       
    }

    $scope.GetIMPSAwaitingData = function () {

        $scope.param = null;
        var item = {};
        item.ESBTab = 1;
        item.TType = 'Awaiting';
        item.TransactionName = 'IMPS';
        item.DateTime = dt;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessFailedAwaiting";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.IMPSAwaitingResponse = JSON.parse(data.ESBGrid);
                
                if ($scope.IMPSAwaitingResponse.length == 0) {
                    // alert('Records Not Found');
                }
            }
        });
       
    }

    $scope.GetBeneFailedData = function () {

        $scope.param = null;
        var item = {};
        item.ESBTab = 2;
        item.TType = 'Failed';
        item.TransactionName = 'Bene verification';
        item.DateTime = dt;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessFailedAwaiting";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.BeneFailedResponse = JSON.parse(data.ESBGrid);
                
                if ($scope.BeneFailedResponse.length == 0) {
                    // alert('Records Not Found');
                }
            }
        });
      
    }

    $scope.GetBeneAwaitingData = function () {

        $scope.param = null;
        var item = {};
        item.ESBTab = 2;
        item.TType = 'Awaiting';
        item.TransactionName = 'Bene verification';
        item.DateTime = dt;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessFailedAwaiting";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.BeneAwaitingResponse = JSON.parse(data.ESBGrid);
                
                if ($scope.BeneAwaitingResponse.length == 0) {
                    //  alert('Records Not Found');
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