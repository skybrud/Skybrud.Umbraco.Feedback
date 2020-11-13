angular.module("umbraco").controller("Skybrud.Feedback.SelectResponsibleController", function ($scope, $http) {

    $scope.loading = false;
    $scope.submitButtonState = "init";

    $scope.entry = $scope.model.entry;
    $scope.users = $scope.model.users.slice();

    $scope.users.unshift({ name: "Ingen ansvarlig", key: "00000000-0000-0000-0000-000000000000" });

    $scope.users.forEach(function(user) {

        user.selected = $scope.entry.assignedTo ? $scope.entry.assignedTo.key === user.key : user.key === "00000000-0000-0000-0000-000000000000";

        if (user.key === "00000000-0000-0000-0000-000000000000") return;
        user.initials = user.name.split(" ").map(x => x.substr(0, 1)).splice(0, 2).join("");

        user.avatarStyles = "";

        if (user.avatar) user.avatarStyles = "background-image: url(" + user.avatar +");";

    });

    $scope.select = function (user) {
        $scope.users.forEach(function (u) {
            u.selected = user && user.key === u.key;
        });
    }

    $scope.save = function () {

        $scope.submitButtonState = "busy";

        var status = $scope.users.find(x => x.selected);

        var data = {
            entry: $scope.entry.key,
            responsible: status.key
        };

        $http.post("/umbraco/backoffice/Skybrud/FeedbackAdmin/SetResponsible", data).then(function (res) {

            $scope.model.entry = res.data;

            $scope.submitButtonState = "success";

            $scope.model.submit($scope.model);

        }, function() {

            $scope.submitButtonState = "error";

        });

    }

});