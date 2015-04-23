angular.module("umbraco").controller("Skybrud.Feedback.SelectResponsibleController", function ($scope, $http, $q, notificationsService, feedbackService) {

    if (!$scope.dialogData) {
        return;
    }

    $scope.loading = true;
    $scope.entry = null;
    $scope.users = null;

    var http1 = feedbackService.getEntry($scope.dialogData.id).success(function (r) {
        $scope.entry = r.data;
    });

    var http2 = feedbackService.getUsers().success(function (r) {
        $scope.users = r.data;
    });
    
    $q.all([http1, http2]).then(function () {
        $scope.loading = false;
    }, function () {
        alert('ERROR');
    });

    $scope.selectResponsible = function (user) {
        $scope.submit(user);
    };

});