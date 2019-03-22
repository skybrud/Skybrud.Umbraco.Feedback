angular.module("umbraco").controller("Skybrud.Feedback.DeleteAllOverlayController", function ($scope) {

    var property = {
        label: "Vælg dato",
        description: "Alle besvarelser på eller før den valgte dato vil blive slettet.",
        view: "datepicker",
        validation: {
            mandatory: true
        },
        config: {
            pickDate: true,
            pickTime: false,
            useSeconds: false,
            format: "YYYY-MM-DD"
        }
    };

    $scope.properties = [property];

    $scope.$watch(function() {
        return property.value;
    }, function(value) {
        $scope.model.date = value;
    });

});