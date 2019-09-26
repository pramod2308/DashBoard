var app = angular.module('app').controller('RefireCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'Logdatetime';
    $scope.sortReverse = true;

    $scope.toDateDisable = true;
    $scope.FromdateRefire = "";
    $scope.todateRefire = "";

    $(window).load(function () {
        $scope.changedValue();
        $scope.changedValue2();
    });

    function ValidateDate(inputdate) {

        //check format of date
        var isvalid = true;
        if (inputdate != "" && inputdate != undefined) {

            var regex = /^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$/;
            if (!(regex.test(inputdate))) {
                isvalid = false;
            }
            else {
                // Check future date validation
                var today = new Date()
                var dateslice = inputdate.split("/");
                var enterdate = new Date(dateslice[2], dateslice[1] - 1, dateslice[0]);
                if (today - enterdate < 0) {
                    isvalid = false;
                }
                else {
                    isvalid = true;
                }
            }
        }

        return isvalid;
    }

    $scope.focusOnFromDate = function () {
        var Todate = $('.Todate');
        var minDate = $('.Fromdate').datepicker('getDate');
        Todate.datepicker('option', 'minDate', minDate);
        $('#dtfromdate').focus();
    }

    $scope.focusOnToDate = function () {
        $('#dttodate').focus();
    }

    $(function () {
        $('#dtfromdate,#dttodate').each(function () {
            $(this).datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: "dd/mm/yy",
                maxDate: new Date(),
                yearRange: '2000:' + (new Date).getFullYear()
            });
        });
    });

    $scope.CheckFromDatevalidation = function () {
        if (!(ValidateDate($scope.Fromdate))) {
            $scope.Fromdate = "";
            alert("Enter valid date.");
            $scope.toDateDisable = true;
        }
        else {
            var date = $scope.Fromdate;
            var fdatearray = date.split("/");
            $scope.FromdateRefire = fdatearray[2] + '-' + fdatearray[1] + '-' + fdatearray[0];
            $scope.toDateDisable = false;
        }
    }

    $scope.CheckToDatevalidation = function () {
        if (!(ValidateDate($scope.Todate))) {
            $scope.todate = "";
            alert("Enter valid From date.");
        }
        else {
            var date = $scope.Todate;
            var tdatearray = date.split("/");
            $scope.todateRefire = tdatearray[2] + '-' + tdatearray[1] + '-' + tdatearray[0];

            var sdate = new Date($scope.Fromdate);
            var edate = new Date($scope.Todate);
            if (edate < sdate) {
                alert("Enter valid To date.");
                $scope.todate = "";
            }
        }
    }

    $scope.changedValue = function (item) {
        var item = {};
        item.fromdaterefire = $scope.FromdateRefire;
        item.todaterefire = $scope.todateRefire;
        item.refire = 1;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/Processrefire";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.Response = JSON.parse(data.dtrefireresponse);
            }
        });

    };

    $scope.Pending = 0;
    $scope.Success = 0;
    $scope.AlreadyProcess = 0;
    $scope.Retried = 0;
    $scope.Total = 0;

    $scope.changedValue2 = function () {
        var item = {};
        item.fromdaterefire = $scope.FromdateRefire;
        item.todaterefire = $scope.todateRefire;
        item.refire = 3;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/Processrefire";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.Response1 = JSON.parse(data.dtrefireresponse);
                for (var i = 0; i < $scope.Response1.length; i++) {
                    $scope.Pending = $scope.Pending + $scope.Response1[i]["Pending"];
                    $scope.Success = $scope.Success + $scope.Response1[i]["Success"];
                    $scope.AlreadyProcess = $scope.AlreadyProcess + $scope.Response1[i]["AlreadyProcess"];
                    $scope.Retried = $scope.Retried + $scope.Response1[i]["Retried"];
                    $scope.Total = $scope.Total + $scope.Response1[i]["Total"];
                }
            }
        });

    };

    $scope.fetchdata = function () {
        $scope.changedValue();
        $scope.changedValue2();
    }

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'Logdatetime';
        $scope.sortReverse = !ascdesc;
    }

    window.setInterval(function () {
        location.reload();
    }, 120000);

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