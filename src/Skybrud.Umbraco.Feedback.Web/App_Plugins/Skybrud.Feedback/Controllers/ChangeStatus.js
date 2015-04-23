angular.module("umbraco").controller("Skybrud.Feedback.ChangeStatusController", function ($scope, $http, $q, notificationsService, feedbackService) {

    if (!$scope.dialogData) {
        return;
    }

    $scope.loading = true;
    $scope.entry = null;
    $scope.statuses = null;

    var http1 = feedbackService.getEntry($scope.dialogData.id).success(function (r) {
        $scope.entry = r.data;
    });

    var http2 = feedbackService.getStatuses().success(function (r) {
        $scope.statuses = r.data;
    });
    
    $q.all([http1, http2]).then(function () {
        $scope.loading = false;
    }, function () {
        alert('ERROR');
    });

    $scope.selectStatus = function (status) {
        $scope.submit(status);
    };

});