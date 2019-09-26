var app = angular.module('app').controller('COLAReportCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {

    $scope.datewisediv = false;
    $scope.toDateDisable = true;

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
        var Todate = $('#dttodate');
        var minDate = $('#dtfromdate').datepicker('getDate');
        Todate.datepicker('option', 'minDate', minDate);
        $scope.toDateDisable = false;
        $('#dtfromdate').focus();
    }

    $scope.focusOnToDate = function () {
        $('#dttodate').focus();
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
            alert("Enter valid From date.");
        }
        else {
            var date = $scope.Fromdate;
            var fdatearray = date.split("/");
            $scope.FromdateCola = fdatearray[2] + '-' + fdatearray[1] + '-' + fdatearray[0];
        }
    }

    $scope.CheckToDatevalidation = function () {
        if ($scope.Todate) {
            if (!(ValidateDate($scope.Todate))) {
                $scope.Todate = "";
                alert("Enter valid To date.");
                return false;
            }
            else {
                var date = $scope.Todate;
                var datearray = date.split("/");
                $scope.todateCola = datearray[2] + '-' + datearray[1] + '-' + datearray[0];

                var sdate = new Date($scope.FromdateCola);
                var edate = new Date($scope.todateCola);

                if (edate < sdate) {
                    alert("Enter valid To date.");
                    $scope.txtToDate = "";
                    return false;
                }
            }
        }
        if ($scope.Todate != undefined) {
            $scope.changedValue2();
        }
    }

    $scope.changedValue = function (item) {

        $scope.param = null;
        var item = {};
        item.COLAID = '1';
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/CashBazaarResponse";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.dtsuryodayresp == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response = JSON.parse(data.CashBazaarGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };

    $scope.changedValue2 = function () {
        $scope.datewisediv = true;
        $scope.param = null;
        var item = {};
        item.COLAID = '3';
        item.FromDate = $scope.FromdateCola;
        item.ToDate = $scope.todateCola;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/CashBazaarResponse";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');

                if (data.dtsuryodayresp == '') {
                    alert('Incorrect Sql Query');
                }

                $scope.Response1 = JSON.parse(data.CashBazaarGrid);

                if ($scope.Response1.length == 0) {
                    alert('Records Not Found');
                }

            }
        });
    };
    $scope.changedValue();

    //    window.setInterval(function () {
    //        location.reload();
    //    }, 120000);
} ]);
