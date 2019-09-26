var app = angular.module('app').controller('SuryodayCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {

    $scope.sortType = 'Local_Date_Time';
    $scope.sortReverse = false;
    $scope.FromdateSur = "";

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
            $scope.FromdateSur = fdatearray[2] + '-' + fdatearray[1] + '-' + fdatearray[0];
        }
    }

    $scope.GetData = function () {
        $scope.changedValue();
        $scope.changedValue2();
    }
    $scope.changedValue = function (item) {

        $scope.param = null;
        var item = {};
        item.EsbTab = 1;
        item.DateTime = $scope.FromdateSur;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessSuryoday";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.dtsuryodayresp == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response = JSON.parse(data.dtsuryodayresp);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };

    $scope.changedValue2 = function (item) {

        $scope.param = null;
        var item = {};
        item.EsbTab = 2;
        item.DateTime = $scope.FromdateSur;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessSuryoday";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.dtsuryodayresp == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response1 = JSON.parse(data.dtsuryodayresp);

                if ($scope.Response1.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };
    $scope.changedValue();
    $scope.changedValue2();

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'Local_Date_Time';
        $scope.sortReverse = !ascdesc;
    }

    //    window.setInterval(function () {
    //        location.reload();
    //    }, 120000);
} ]);
