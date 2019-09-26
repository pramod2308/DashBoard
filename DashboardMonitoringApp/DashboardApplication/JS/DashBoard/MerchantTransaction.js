var app = angular.module('app').controller('MerchantCtrl', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'appid';
    $scope.sortReverse = false;
    $scope.mer = true;

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
    $scope.changedStart = function (start) {
        STime = start.value;
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

    $scope.changedEnd = function (end) {
        ETime = end.value;
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

    $scope.ExportToCSV = function () {

        var data = $scope.Response;
        if (data == '')
            return;

        JSONToCSVConvertor(data, "Report", true);

    }

    function JSONToCSVConvertor(JSONData, ReportTitle, ShowLabel) {
        //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
        var arrData = typeof JSONData != 'object' ? JSON.parse(JSONData) : JSONData;

        var CSV = '';
        //Set Report title in first row or line

        // CSV += ReportTitle + '\r\n\n';

        //This condition will generate the Label/Header
        if (ShowLabel) {
            var row = "";

            //This loop will extract the label from 1st index of on array
            for (var index in arrData[0]) {

                //Now convert each value to string and comma-seprated
                row += index + ',';
            }

            row = row.slice(0, -1);

            //append Label row with line break
            CSV += row + '\r\n';
        }

        //1st loop is to extract each row
        for (var i = 0; i < arrData.length; i++) {
            var row = "";

            //2nd loop will extract each column and convert it in string comma-seprated
            for (var index in arrData[i]) {
                row += '"' + arrData[i][index] + '",';
            }

            row.slice(0, row.length - 1);

            //add a line break after each row
            CSV += row + '\r\n';
        }

        if (CSV == '') {
            alert("Invalid data");
            return;
        }

        //Generate a file name
        var fileName = "MerchantTransaction";
        //this will remove the blank-spaces from the title and replace it with an underscore
        fileName += ReportTitle.replace(/ /g, "_");

        //Initialize file format you want csv or xls
        var uri = 'data:text/csv;charset=utf-8,' + escape(CSV);

        // Now the little tricky part.
        // you can use either>> window.open(uri);
        // but this will not work in some browsers
        // or you will not get the correct file extension    

        //this trick will generate a temp <a /> tag
        var link = document.createElement("a");
        link.href = uri;

        //set the visibility hidden so it will not effect on your web-layout
        link.style = "visibility:hidden";
        link.download = fileName + ".csv";

        //this part will append the anchor tag and remove it after automatic click
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    
    function changedValue(changedvalue) {        
        if (changedvalue == 15 || changedvalue == null) {
            STime = "";
            ETime = "";
        }

        $scope.param = null;
        var item = {};
        item.MerchantTab = 1;
        item.Sdattime = STime;
        item.Edattime = ETime;
        item.UserID = '';
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessMerchantTransaction";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut();
                $scope.Response = JSON.parse(data.MerchantGrid);

                 $("#MerchantData").kendoGrid({
                    //                    toolbar: ["excel"],
                    //                    excel: {
                    //                        fileName: "Test.xlsx",
                    //                        proxyURL: "https://demos.telerik.com/kendo-ui/service/export",
                    //                        filterable: true
                    //                    },
                    dataSource: {
                        data: $scope.Response,
                        schema: {
                            model: {
                                fields: {
                                    appid: { type: "string" },
                                    UserID: { type: "string" },
                                    TransactionType: { type: "string" },
                                    Initiated: { type: "string" },
                                    Success: { type: "string" },
                                    Failed: { type: "string" },
                                    Awaiting: { type: "string" },
                                    Amount: { type: "string" }
                                }
                            }
                        },
                        pageSize: 10
                      //  Filtering: true
                    },
                    height: 450,
                    scrollable: true,
                    // sortable: true,  
//                    filterable: {
//                            mode: "row"
//                        },   
                        pageable: {
                        pageSize: $scope.gridPageSize,
                        input: true,
                        numeric: true,
                        pageSizes: [10, 25, 50, 100, 'All']
                    }
//                        columns: 
//                        [{
//                            field: "appid",                          
//                            filterable: {
//                                cell: {
//                                    operator: "contains",
//                                    suggestionOperator: "contains",
//                                    showOperators: false
//                                }
//                            }
//                        },
//                        {
//                            field: "UserID",
//                            filterable: {
//                                cell: {
//                                    showOperators: false
//                                }
//                            }                            
//                        },
//                        {
//                            field: "TransactionType",
//                            filterable: {
//                                cell: {
//                                    showOperators: false
//                                }
//                            }                           
//                        },
//                        {
//                            field: "Initiated",
//                            filterable: {
//                                cell: {
//                                    showOperators: false
//                                }
//                            }                           
//                        },
//                        {
//                            field: "Success",
//                            filterable: {
//                                cell: {
//                                    showOperators: false
//                                }
//                            }                         
//                        },
//                        {
//                            field: "Failed",
//                            filterable: {
//                                cell: {
//                                    showOperators: false
//                                }
//                            }                          
//                        },
//                        {
//                            field: "Awaiting",
//                            filterable: {
//                                cell: {
//                                    showOperators: false
//                                }
//                            }                           
//                        },
//                        {
//                            field: "Amount",
//                            filterable: {
//                                cell: {
//                                    showOperators: false
//                                }
//                            }                           
//                        }]                                                     
                });
                //   if ($scope.Response.length == 0) {
                //        alert('Records Not Found');
                //   }
            }
        });

    };

//    // Find the Role filter menu.
//    var filterMenu = _grid.thead.find("th:not(.k-hierarchy-cell,.k-group-cell):first").data("kendoFilterMenu");

//    filterMenu.form.find("div.k-filter-help-text").text("Select APP ID:");
//    filterMenu.form.find("span.k-dropdown:first").css("display", "none");

//    // Change the text field to a dropdownlist in the Role filter menu.
//    filterMenu.form.find(".k-textbox:first")
//    .removeClass("k-textbox")
//    .kendoDropDownList({
//        dataSource: new kendo.data.DataSource({
//            data: [
//                { title: "FINOMB" },
//                { title: "FINOTLR" },
//                { title: "MB" }
//            ]
//        }),
//        dataTextField: "title",
//        dataValueField: "title"
//    });

    $("#txtSearchString").keyup(function () {
        var val = $('#txtSearchString').val();
        $("#MerchantData").data("kendoGrid").dataSource.filter({
            logic: "or",
            filters: [{ field: "appid", operator: "contains", value: val },
                      { field: "UserID", operator: "contains", value: val },
                      { field: "TransactionType", operator: "contains", value: val },
                      { field: "Initiated", operator: "contains", value: val },
                      { field: "Success", operator: "contains", value: val },
                      { field: "Failed", operator: "contains", value: val },
                      { field: "Awaiting", operator: "contains", value: val },
                      { field: "Amount", operator: "contains", value: val }
                     ]
        });

    });

        $("#ddlappid").change(function () {
            var val = $('#ddlappid').val();
            $("#MerchantData").data("kendoGrid").dataSource.filter({
                logic: "or",
                filters: [{ field: "appid", operator: "contains", value: val }]
            });

        });

    $scope.GetAllrecords = function () {

        if (STime == '' || ETime == '') {
            STime = "";
            ETime = "";
        }
        $scope.param = null;
        var item = {};
        item.MerchantTab = 2;
        item.Sdattime = STime;
        item.Edattime = ETime;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/DashboardAppService.svc/ProcessMerchantTransaction";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.Response2 = JSON.parse(data.MerchantGrid);

                if ($scope.Response2.length == 0) {
                    alert('Records Not Found');
                }
            }
        });
        $scope.mer = false;
    }

    $scope.GetFailedData = function (TraName) {
        debugger;
        var TransName = TraName;
        if (TransName == 'Bene verification') {
            $('#IMPSWalk-In').removeClass('in');
            $('#NEFT').removeClass('in');
            $('#IMPSWalk-In1').removeClass('in');
        }
        else if (TransName == 'IMPS Walk-In') {
            $('#Beneverification').removeClass('in');
            $('#NEFT').removeClass('in');
            $('#Beneverification1').removeClass('in');
        }
        else {
            $('#Beneverification').removeClass('in');
            $('#IMPSWalk-In').removeClass('in');
            $('#Beneverification1').removeClass('in');
            $('#IMPSWalk-In1').removeClass('in');
        }

        $scope.param = null;
        var item = {};
        item.ESBTab = 2;
        item.TType = 'Failed';
        item.TransactionName = TransName;
        item.Sdattime = STime;
        item.Edattime = ETime;
        item.appid = 'FINOMER';
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

    $scope.GetAwaitingData = function (TraName) {
        var TransName = TraName;

        if (TransName == 'Bene verification') {
            $('#IMPSWalk-In').removeClass('in');
            $('#NEFT').removeClass('in');
            $('#IMPSWalk-In1').removeClass('in');
        }
        else {
            $('#Beneverification').removeClass('in');
            $('#NEFT').removeClass('in');
            $('#Beneverification1').removeClass('in');
        }


        $scope.param = null;
        var item = {};
        item.ESBTab = 3;
        item.TType = 'Awaiting';
        item.TransactionName = TransName;
        item.Sdattime = STime;
        item.Edattime = ETime;
        item.appid = 'FINOMER';
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

    $scope.Cancel = function () {
        $scope.mer = true;
    }


    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'appid';
        $scope.sortReverse = !ascdesc;
    }

    window.setInterval(function () {
        location.reload();
    }, 120000);

} ]);