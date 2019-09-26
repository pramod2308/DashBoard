var app = angular.module('app').controller('ctrlHAESBLatency', ['$scope', '$http', 'ServiceCall', '$log', '$window', 'cache', '$timeout', function ($scope, $http, ServiceCall, $log, $window, cache, $timeout) {

    $scope.sortType = 'SERVER1_AVG_Time';   
    $scope.sortReverse = true;

    $scope.Option = [
    { id: 1, Time: 'Last 10 Minute', value: '10' },
    { id: 2, Time: 'Last 15 Minute', value: '15' },
    { id: 3, Time: 'Last 30 Minute', value: '30' },
    { id: 4, Time: 'Last 1 Hour', value: '1' },
    { id: 5, Time: 'Today', value: 'Today' }
];

    $(window).load(function () {
        $scope.changedValue(2);
    });

    $scope.changedValue = function (item) {
        if (item == 2 || item == null) {
            var DatTime = 2;
        }
        else {
            var DatTime = item.value;
        }

        var item = {};
        item.DateTime = DatTime;
        $scope.param = item;
        $('#loader').fadeIn();
        $scope.URL = "/PBDashboardApp/DashboardAppService.svc/ProcessHA_ESBLatency";
        ServiceCall.PostData($scope.URL, $scope.param).then(function (data) {
            if (data != null || data != undefined) {
                $('#loader').fadeOut('slow');
                $scope.Response = JSON.parse(data.LatencyGrid);

                if ($scope.Response.length == 0) {
                    alert('Records Not Found');
                }
            }
        });
    }
    //                $("#ESBLatencyData").kendoGrid({
    //                    dataSource: {
    //                        data: $scope.Response, 
    //                        schema: {
    //                            model: {
    //                                fields: {
    //                                    URL: { type: "string" },
    //                                    SERVER_Total_Hits: { type: "string" },
    //                                    ServerName: { type: "string" },
    //                                    SERVER1_AVG_Time: { type: "string" },
    //                                    Last_Hits_Time: { type: "string" },
    //                                    Response_200_Success: { type: "string" },
    //                                    Response_400: { type: "string" },
    //                                    Response_401: { type: "string" },
    //                                    Response_404: { type: "string" },
    //                                    Response_405: { type: "string" },
    //                                    Response_500: { type: "string" },
    //                                    Response_502: { type: "string" },
    //                                    Response_503: { type: "string" },
    //                                    Response_504: { type: "string" }
    //                                }
    //                            }
    //                        },                      
    //                        pageSize: 25
    //                    },
    //                    height: 450,
    //                    sortable: true,
    //                    scrollable: true,
    //                    pageable: {
    //                        pageSize: $scope.gridPageSize,
    //                        input: true,
    //                        numeric: true,
    //                        pageSizes: [10, 25, 50, 100, 'All']
    //                    },
    //                    columns: [
    //                            {
    //                                title: "#",
    //                                template: "",
    //                                width: "50px"
    //                            },
    //                            { field: "URL", width: "250px" },
    //                            { field: "SERVER_Total_Hits", title: "TotalHits", width: "100px" },
    //                            { field: "ServerName", title: "ServerName", width: "100px" },
    //                            { field: "SERVER1_AVG_Time", title: "AVG_Time", width: "100px" },
    //                            { field: "Last_Hits_Time", title: "Last_Hits_Time", width: "120px" },
    //                            { field: "Response_200_Success", title: "200" },
    //                            { field: "Response_400", title: "400" },
    //                            { field: "Response_401", title: "401" },
    //                            { field: "Response_404", title: "404" },
    //                            { field: "Response_405", title: "405" },
    //                            { field: "Response_500", title: "500" },
    //                            { field: "Response_502", title: "502" },
    //                            { field: "Response_503", title: "503" },
    //                            { field: "Response_504", title: "504" }
    //                        ],                   
    //                });
    //            }
    //        });        
    //  };

//    $("#search_field").keyup(function () {
//        var val = $('#search_field').val();
//        $("#ESBLatencyData").data("kendoGrid").dataSource.filter({
//            logic: "or",
//            filters: [{ field: "URL", operator: "contains", value: val },
//                      { field: "SERVER_Total_Hits", operator: "contains", value: val },
//                      { field: "ServerName", operator: "contains", value: val },
//                      { field: "SERVER1_AVG_Time", operator: "contains", value: val },
//                      { field: "Last_Hits_Time", operator: "contains", value: val },
//                      { field: "Response_200_Success", operator: "contains", value: val },
//                      { field: "Response_400", operator: "contains", value: val },
//                      { field: "Response_401", operator: "contains", value: val },
//                      { field: "Response_404", operator: "contains", value: val },
//                      { field: "Response_405", operator: "contains", value: val },
//                      { field: "Response_500", operator: "contains", value: val },
//                      { field: "Response_502", operator: "contains", value: val },
//                      { field: "Response_503", operator: "contains", value: val },
//                      { field: "Response_504", operator: "contains", value: val }
//                     ]
//        });

//    });

    $scope.Sorting = function () {
        var ascdesc = $scope.sortReverse;
        $scope.sortType = 'SERVER1_AVG_Time';       
        $scope.sortReverse = !ascdesc;
    }

    window.setInterval(function () {
        location.reload();
    }, 120000);

} ]);
