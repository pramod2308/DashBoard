var app = angular.module('app').controller('ESBLatencyReportCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'Service_name';
    $scope.sortReverse = false;

    $scope.SelectTime = [
    { id: 1, Time: '01', value: '01' },
    { id: 2, Time: '02', value: '02' },
    { id: 1, Time: '03', value: '03' },
    { id: 2, Time: '04', value: '04' },
    { id: 1, Time: '05', value: '05' },
    { id: 2, Time: '06', value: '06' },
    { id: 1, Time: '07', value: '07' },
    { id: 2, Time: '08', value: '08' },
    { id: 1, Time: '09', value: '09' },
    { id: 1, Time: '10', value: '10' },
    { id: 2, Time: '11', value: '11' },
    { id: 1, Time: '12', value: '12' },
    { id: 2, Time: '13', value: '13' },
    { id: 2, Time: '14', value: '14' },
    { id: 1, Time: '15', value: '15' },
    { id: 2, Time: '16', value: '16' },
    { id: 1, Time: '17', value: '17' },
    { id: 2, Time: '18', value: '18' },
    { id: 1, Time: '19', value: '19' },
    { id: 2, Time: '20', value: '20' },
    { id: 1, Time: '21', value: '21' },
    { id: 2, Time: '22', value: '22' },
    { id: 1, Time: '23', value: '23' },
    { id: 1, Time: '24', value: '24' }
];

    $(window).load(function () {
        changedValue(2);
    });

    var STime = '', ETime = '';
    $scope.changedStart = function (item) {
        STime = item.value;
        if (ETime != '') {
            if (STime >= ETime) {
                alert('Start time should be less than end time');
                return false;
            }
            else {
                changedValue(1);
            }
        }
    }

    $scope.changedEnd = function (item) {
        ETime = item.value;
        if (STime != '') {
            if (STime >= ETime) {
                alert('End time should be greater than Start time');
                return false;
            }
            else {
                changedValue(1);
            }
        }
    }

    $scope.chgtoday = function (today) {
        if (today == true) {
            STime = 'today';
            ETime = 'today';
            changedValue(1);
        }
        else {
            changedValue(2);
        }

    }

    function changedValue(item) {
        if (item == 2 || item == null) {
            STime = "";
            ETime = "";
        }

        var item = {};
        item.LatencyType = 'ESBLatencyReport';
        item.Sdattime = STime;
        item.Edattime = ETime;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/PBDashboardApp/DashboardAppService.svc/ProcessESBLatency";
        
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.Response = JSON.parse(data.ESBLatencyGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'TotalHits';
        $scope.sortReverse = !ascdesc;
    }

        window.setInterval(function () {
            location.reload();
        }, 120000);

} ]);
