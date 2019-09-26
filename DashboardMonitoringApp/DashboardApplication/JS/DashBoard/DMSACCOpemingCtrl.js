var app = angular.module('app').controller('DMSAccOpeningCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache,$timeout) {

    $scope.sortType = 'newproductdesc';
    $scope.sortReverse = false;
    $scope.DMSDate = "";

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

    $scope.focusOnFromDate = function () {
        $('#dtfromdate').focus();
    }

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

    $scope.CheckFromDatevalidation = function () {
        if (!(ValidateDate($scope.Fromdate))) {
            $scope.Fromdate = "";
            alert("Enter valid date.");
        }
        else {
            var date = $scope.Fromdate;
            var fdatearray = date.split("/");
            $scope.DMSDate = fdatearray[2] + '-' + fdatearray[1] + '-' + fdatearray[0];
        }
    }

    $scope.GetData = function () {
        $scope.changedValue();
    }

    $scope.allDVU_count = 0;
    $scope.DVU_TOTAL = 0;
    $scope.DMS_ACCEPT = 0;
    $scope.DMS_REVISION = 0;
    $scope.DMS_DISCARD = 0;
    $scope.DMS_DEVIATION = 0;
    $scope.Cal = 0;
    $scope.ALLTOTAL = 0;

    $scope.changedValue = function (item) {

        $scope.allDVU_count = 0;
        $scope.DVU_TOTAL = 0;
        $scope.DMS_ACCEPT = 0;
        $scope.DMS_REVISION = 0;
        $scope.DMS_DISCARD = 0;
        $scope.DMS_DEVIATION = 0;
        $scope.AllTotal = 0;

        $scope.param = null;
        var item = {};
        item.DMSDateTime = $scope.DMSDate;
        item.Tab = "AODashBoard";
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessDMSAccountOpening";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.dtsuryodayresp == '') {
                    alert('Incorrect Sql Query');
                }
                $scope.Response = JSON.parse(data.DMSDashboardGrid);

                for (var i = 0; i < $scope.Response.length; i++) {
                    $scope.allDVU_count = $scope.allDVU_count + $scope.Response[i]["allDVU_count"];
                    $scope.DVU_TOTAL = $scope.DVU_TOTAL + $scope.Response[i]["DVU_TOTAL"];
                    $scope.DMS_ACCEPT = $scope.DMS_ACCEPT + $scope.Response[i]["DMS_ACCEPT"];
                    $scope.DMS_REVISION = $scope.DMS_REVISION + $scope.Response[i]["DMS_REVISION"];
                    $scope.DMS_DISCARD = $scope.DMS_DISCARD + $scope.Response[i]["DMS_DISCARD"];
                    $scope.DMS_DEVIATION = $scope.DMS_DEVIATION + $scope.Response[i]["DMS_DEVIATION"];
                    $scope.Response[i].AllTotal = ($scope.Response[i]["allDVU_count"] + $scope.Response[i]["DVU_TOTAL"]) - ($scope.Response[i]["DMS_ACCEPT"] + $scope.Response[i]["DMS_REVISION"] + $scope.Response[i]["DMS_DISCARD"] + $scope.Response[i]["DMS_DEVIATION"]);
                }

                
                $scope.FinalResponse = $scope.Response;

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };

    $scope.changedValue2 = function () {

        $scope.param = null;
        var item = {};
        item.DMSDateTime = $scope.DMSDate;
        item.Tab = "AODashBoard_pending";
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessDMSAccountOpening";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.dtsuryodayresp == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response2 = JSON.parse(data.DMSDashboardGrid);

                if ($scope.Response2.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };
    $scope.changedValue();
    $scope.changedValue2();

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'newproductdesc';
        $scope.sortReverse = !ascdesc;
    }

    //    window.setInterval(function () {
    //        location.reload();
    //    }, 120000);
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