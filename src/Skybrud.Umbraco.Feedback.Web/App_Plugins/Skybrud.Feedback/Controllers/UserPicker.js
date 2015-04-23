angular.module("umbraco").controller("Skybrud.Feedback.UserPickerController", function ($scope, dialogService) {

    $scope.select = function () {
        dialogService.closeAll();
        dialogService.open({
            template: '/App_Plugins/Skybrud.Feedback/Views/UserPickerDialog.html',
            modalClass: 'umb-modal feedback-modal',
            callback: function (user) {
                $scope.model.value = user;
            }
        });
    };

    $scope.clear = function () {
        $scope.model.value = null;
    };

});