var app = angular.module('app').controller('FifthDashBoardCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', function ($scope, $http, ServiceCall, $log, $window, cache) {

    $scope.sortType = 'appid';
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

    $scope.Initiated = 0;
    $scope.Success = 0;
    $scope.Failed = 0;
    $scope.Awaiting = 0;
    function changedValue(item) {

        if (item == 2 || item == null) {
            STime = "";
            ETime = "";
        }

        $scope.param = null;
        var item = {};
        item.ESBTab = 1;
        item.Sdattime = STime;
        item.Edattime = ETime;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessESBRequest";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.Response = JSON.parse(data.ESBGrid);
                for (var i = 0; i < $scope.Response.length; i++) {
                    $scope.Initiated = $scope.Initiated + $scope.Response[i]["Initiated"];
                    $scope.Success = $scope.Success + $scope.Response[i]["Success"];
                    $scope.Failed = $scope.Failed + $scope.Response[i]["Failed"];
                    $scope.Awaiting = $scope.Awaiting + $scope.Response[i]["Awaiting"];
                }
                $scope.SelectAppId = Response.appid;
                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }
            }
        });

    };

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'appid';
        $scope.sortReverse = !ascdesc;
    }

    window.setInterval(function () {        
        location.reload();
    }, 120000);

    $scope.GetFailedData = function (TraName, appid) {
        var TransName = TraName;
        if (appid == 'FINOTLR') {
            if (TransName == 'Bene verification') {
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else if (TransName == 'IMPS Walk-In') {
                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else {
                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
        }
        else if (appid == 'FINOMER') {
            if (TransName == 'Bene verification') {

                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else if (TransName == 'IMPS Walk-In') {
                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else {
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
        }
        else if (appid == 'FINOMERNP') {
            if (TransName == 'Bene verification') {

                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');
            }
            else if (TransName == 'IMPS Walk-In') {
                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');
            }
            else {
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');
            }
        }
        else {
            if (TransName == 'Bene verification') {

                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else if (TransName == 'IMPS Walk-In') {
                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else {
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
        }

        $scope.param = null;
        var item = {};
        item.ESBTab = 2;
        item.TType = 'Failed';
        item.TransactionName = TransName;
        item.appid = appid;
        item.Sdattime = STime;
        item.Edattime = ETime;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessESBRequest";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.FailedResponse = JSON.parse(data.ESBGrid);
                if ($scope.FailedResponse.length == 0) {
                    // alert('Records Not Found');
                }
            }
        });

    }

    $scope.GetAwaitingData = function (TraName, appid) {
        var TransName = TraName;

        var TransName = TraName;
        if (appid == 'FINOTLR') {
            if (TransName == 'Bene verification') {
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else if (TransName == 'IMPS Walk-In') {
                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else {
                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
        }
        else if (appid == 'FINOMER') {
            if (TransName == 'Bene verification') {

                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else if (TransName == 'IMPS Walk-In') {
                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else {
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
        }
        else if (appid == 'FINOMERNP') {
            if (TransName == 'Bene verification') {

                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');
            }
            else if (TransName == 'IMPS Walk-In') {
                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');
            }
            else {
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');
            }
        }
        else {
            if (TransName == 'Bene verification') {

                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else if (TransName == 'IMPS Walk-In') {
                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBNEFT').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
            else {
                $('#FINOMBIMPSWalk-In').removeClass('in');
                $('#FINOMBBeneverification').removeClass('in');
                $('#FINOMBBeneverification1').removeClass('in');
                $('#FINOMBIMPSWalk-In1').removeClass('in');

                $('#FINOTLRBeneverification').removeClass('in');
                $('#FINOTLRBeneverification1').removeClass('in');
                $('#FINOTLRIMPSWalk-In').removeClass('in');
                $('#FINOTLRNEFT').removeClass('in');
                $('#FINOTLRIMPSWalk-In1').removeClass('in');

                $('#FINOMERBeneverification').removeClass('in');
                $('#FINOMERBeneverification1').removeClass('in');
                $('#FINOMERIMPSWalk-In').removeClass('in');
                $('#FINOMERNEFT').removeClass('in');
                $('#FINOMERIMPSWalk-In1').removeClass('in');

                $('#FINOMERNPBeneverification').removeClass('in');
                $('#FINOMERNPBeneverification1').removeClass('in');
                $('#FINOMERNPIMPSWalk-In').removeClass('in');
                $('#FINOMERNPNEFT').removeClass('in');
                $('#FINOMERNPIMPSWalk-In1').removeClass('in');
            }
        }


        $scope.param = null;
        var item = {};
        item.ESBTab = 3;
        item.TType = 'Awaiting';
        item.TransactionName = TransName;
        item.Sdattime = STime;
        item.Edattime = ETime;
        item.appid = appid;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessESBRequest";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.AwaitingResponse = JSON.parse(data.ESBGrid);
                if ($scope.AwaitingResponse.length == 0) {
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