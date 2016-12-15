angular.module('umbraco').controller('Skybrud.Feedback.FiltersPreValueEditorController', function ($scope, feedbackService) {

    $scope.filters = feedbackService.getFilters();

    if (!$scope.model.value) {
        $scope.model.value = {};
        angular.forEach($scope.filters, function (item) {
            $scope.model.value[item.alias] = true;
        });
    }

});