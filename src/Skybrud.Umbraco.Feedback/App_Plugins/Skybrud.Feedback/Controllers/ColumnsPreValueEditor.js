angular.module('umbraco').controller('Skybrud.Feedback.ColumnsPreValueEditorController', function ($scope, feedbackService) {

    $scope.columns = feedbackService.getColumns();

    if (!$scope.model.value) {
        $scope.model.value = {};
        angular.forEach($scope.columns, function(item) {
            $scope.model.value[item.alias] = true;
        });
    }

});