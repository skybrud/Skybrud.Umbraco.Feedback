angular.module("umbraco").controller("Skybrud.Feedback.UserPickerDialogController", function ($scope, $http, $q, notificationsService, feedbackService) {

    $scope.loading = true;
    $scope.entry = null;
    $scope.users = null;

    feedbackService.getUsers().success(function (r) {

        console.log(r);

        $scope.users = r.data ? r.data : r;
        $scope.loading = false;
    }).error(function (r) {
        notificationsService.error('Feedback: Unable to load users' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));
        $scope.loading = false;
    });

    $scope.select = function (user) {
        $scope.submit(user);
    };

});