angular.module("umbraco").controller("Skybrud.Feedback.SelectStatusController", function ($scope, $http, editorService, editorState, localizationService, notificationsService, userService) {

    $scope.loading = false;
    $scope.submitButtonState = "init";

    $scope.entry = $scope.model.entry;
    $scope.selected = $scope.entry.status;
    $scope.statuses = $scope.entry.site.statuses;

    $scope.statuses.forEach(function(status) {
        status.selected = $scope.selected && $scope.selected.key === status.key;
    });

    $scope.select = function(status) {
        $scope.statuses.forEach(function (s) {
            s.selected = status && status.key === s.key;
        });
    }

    $scope.save = function () {

        $scope.submitButtonState = "busy";

        var status = $scope.statuses.find(x => x.selected);

        var data = {
            entry: $scope.entry.key,
            status: status.key
        };

        $http.post("/umbraco/backoffice/Skybrud/FeedbackAdmin/SetStatus", data).then(function (res) {

            $scope.model.entry = res.data;

            $scope.submitButtonState = "success";

            $scope.model.submit($scope.model);

        }, function() {

            $scope.submitButtonState = "error";

        });

    }

});