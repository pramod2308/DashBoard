
//angular.module('app').config(['$httpProvider', function ($httpProvider) {
////    $httpProvider.defaults.useXDomain = true;
////    delete $httpProvider.defaults.headers.common['X-Requested-With'];
////    $httpProvider.defaults.useXDomain = true;
////    $httpProvider.defaults.withCredentials = true;
////    delete $httpProvider.defaults.headers.common["X-Requested-With"];
// //   $httpProvider.defaults.headers.common["Accept"] = "application/json";
////    $httpProvider.defaults.headers.common["Content-Type"] = "application/json";
////    response.headers['Access-Control-Allow-Origin'] = '*'
////    response.headers['Access-Control-Allow-Methods'] = 'POST, GET, OPTIONS, PUT'
////    response.headers['Access-Control-Allow-Headers'] = 'Origin, X-Requested-With, Content-Type, Accept'
//}
//]);



//angular.module('app').factory('ServiceCall', function ($http, $q) {
//    return {

//        PostData: function (URL, param) {

//            $.ajax({
//                type: "POST",
//                url: URL,
//                data: param,
//                cache: false,
//                contentType: "application/json",
//                dataType: "json",
//                success: function (data) {
//                    alert(data);
//                },
//                error: function (e) {
//                    alert(e);
//                }
//            });


            //            var deferred = $q.defer();
            //            $http({
            //                url: URL,
            //                method: "POST",
            //                data: param, //Data sent to server
            //                contentType: "application/json",
            //                dataType: "json"
            //            }).success(function (data) {
            //                deferred.resolve(data);
            //            }).error(function (e) {
            //                deferred.reject();
            //            });

            //            return deferred.promise;
//        }
//    }
//});

function UniqueRequestId() {

    var d = new Date();
    var month = d.getMonth() + 1;
    var date = d.getDate();
    var year = d.getFullYear();
    var hr = d.getHours();
    var min = d.getMinutes();
    var sec = d.getSeconds();
    var uniqueId = month + "" + date + "" + year + "" + hr + "" + min + "" + sec;

    return uniqueId;
}

angular.module('app').directive('ngConfirmClick', [
  function () {
      return {
          priority: -1,
          restrict: 'A',
          link: function (scope, element, attrs) {
              element.bind('click', function (e) {
                  var message = attrs.ngConfirmClick;
                  // confirm() requires jQuery
                  if (message && !confirm(message)) {
                      e.stopImmediatePropagation();
                      e.preventDefault();
                  }
              });
          }
      }
  }
]);

//  angular.module('app').directive('datatable', function () {
//      return {
//          restrict: 'AC',
//          link: function (scope, element, attrs) {
//              //
//              scope.$watch('skills', function (newValue, oldValue) {
//                  if (newValue != undefined) {
//                      if (newValue.length > 0) {
//                          var rows = element.find("tbody").find('tr');
//                          var fixedColumn = 3;
//                          if ($.fn.dataTable.isDataTable(element)) {
//                              var tbl = $(element).dataTable();
//                              tbl.fnClearTable();
//                              for (var i = 0; i < rows.length; i++) {
//                                  tbl.fnAddData($(rows[i]));
//                              }
//                          }
//                          else {
//                              element.DataTable({ paging: true, sorting: true, "order": [[0, "asc"]], columnDefs: [{ orderable: false, "targets": fixedColumn}] });
//                          }
//                          element.find('tbody').on('click', 'tr', function () {
//                              $(this).addClass('selected');
//                              $(this).siblings('tr').removeClass('selected');
//                          });
//                          element.fadeIn();
//                      }
//                  }
//              }, true);
//          }
//      }
//  });

  angular.module('app').factory('cache', function ($cacheFactory) {
      var cache = $cacheFactory('PBcache');
      return cache;
  });


  //This directive is used to set focus particular text
  angular.module('app').directive('ngFocus', function ($timeout) {
      return {
          link: function (scope, element, attrs) {
              scope.$watch(attrs.ngFocus, function (val) {
                  if (angular.isDefined(val) && val) {
                      $timeout(function () { element[0].focus(); });
                  }
              }, true);

              element.bind('blur', function () {
                  if (angular.isDefined(attrs.ngFocusLost)) {
                      scope.$apply(attrs.ngFocusLost);

                  }
              });
          }
      };
  });


//  app.directive('ngFocus', function ($timeout) {
//      return {
//          link: function (scope, element, attrs) {
//              scope.$watch(attrs.ngFocus, function (val) {
//                  if (angular.isDefined(val) && val) {
//                      $timeout(function () { element[0].focus(); });
//                  }
//              }, true);
//          }
//      };
//  });

  angular.module('app').filter('spaceless', function () {
      return function (input) {
          if (input) {
              input = input.replace('&', '');
              input = input.replace('(', '');
              input = input.replace(')', '');
              input = input.replace('/', '');
              return input.replace(/\s+/g, '');
          }
      }
  });

 angular.module('app').factory("XLSXReaderService", ['$q', '$rootScope',
        function($q, $rootScope) {
            var service = function(data) {
                angular.extend(this, data);
            };

            service.readFile = function(file, showPreview) {
                var deferred = $q.defer();

                XLSXReader(file, showPreview, function(data){
                    $rootScope.$apply(function() {
                        deferred.resolve(data);
                    });
                });

                return deferred.promise;
            };

            return service;
        }
    ]);

angular.module('app').factory('ServiceCall', function ($http, $q) {
    return {

        PostData: function (URL, param) {
            var deferred = $q.defer();
            $http({
                url: URL,
                method: "POST",
                data: param,
                contentType: "application/json; charset=utf-8",
                dataType: "json" 
              
            }).success(function (data) {
                deferred.resolve(data);
            }).error(function (data) {
                deferred.reject();
            });

            return deferred.promise;
        
        }

    }

});