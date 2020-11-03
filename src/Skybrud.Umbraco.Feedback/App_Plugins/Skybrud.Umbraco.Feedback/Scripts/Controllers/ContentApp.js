angular.module("umbraco").controller("Skybrud.Feedback.ContentAppController", function ($scope, $http, editorState, notificationsService) {

    $scope.current = editorState.current;

    $scope.loading = true;
    $scope.entries = null;
    $scope.pagination = null;
    $scope.sorting = {};

    $scope.archive = function (entry, success, error) {
        if (!confirm("Are you sure you want to archive the selected feedback entry?")) return;
        $http.get("/umbraco/backoffice/Skybrud/FeedbackAdmin/Archive?key=" + entry.key).then(function (res) {
            notificationsService.success("The entry was successfully archived.");
            success(res.data);
        }, function (res) {
            notificationsService.error("Unable to archive entry" + (res.data && res.data.meta && res.data.meta.error ? ": " + res.data.meta.error : ""));
            error(entry);
        });

    };

    $scope.delete = function (entry, success, error) {
        if (!confirm("Are you sure you want to delete the selected feedback entry?")) return;
        $http.get("/umbraco/backoffice/Skybrud/FeedbackAdmin/Delete?key=" + entry.key, { umbIgnoreErrors: true }).then(function (res) {
            notificationsService.success("The entry was successfully deleted.");
            success(res.data);
        }, function (res) {
            notificationsService.error("Unable to delete entry" + (res.data && res.data.meta && res.data.meta.error ? ": " + res.data.meta.error : ""));
            error(entry);
        });
    };

    $scope.sort = function (field, order) {

        if (order !== "desc") order = "asc";

        if (field === $scope.sorting.field) {
            $scope.sorting.order = $scope.sorting.order === "asc" ? "desc" : "asc";
        } else {
            $scope.sorting.field = field;
            $scope.sorting.order = order;
        }

        $scope.update();

    };

    $scope.isSortDirection = function (field, order) {
        return field === $scope.sorting.field && order === $scope.sorting.order;
    };

    $scope.prev = function () {
        if ($scope.pagination.page > 1) $scope.update($scope.pagination.page - 1);
    };

    $scope.next = function () {
        if ($scope.pagination.page < $scope.pagination.pages) $scope.update($scope.pagination.page + 1);
    };

    $scope.refresh = function() {
        $scope.update();
    };

    function updateBadge(list) {
        if (list.pagination.total > 0) {
            $scope.model.badge = {
                count: list.pagination.total,
                type: "warning"
            };
        } else {
            $scope.model.badge = null;
        }
    }

    function updatePagination(list) {

        var pagination = list.pagination;
        pagination.pagination = [];

        var page = pagination.page;
        var pages = pagination.pages;

        for (var i = Math.max(1, page - 5); i <= Math.min(page + 5, pages); i++) {
            pagination.pagination.push({
                page: i,
                active: i === page
            });
        }

        $scope.pagination = pagination;

    }

    function updateSorting(list) {
        $scope.sorting = list.sorting;
    }

    function updateButtons(e) {

        if (e.comment) e.$comment = e.comment.trim().split("\n");

        e.archiveButton = {
            defaultButton: {
                labelKey: "feedback_btnArchive",
                handler: function () {
                    e.archiveButton.state = "busy";
                    $scope.archive(e, function () {
                        e.archiveButton.state = "success";
                        $scope.refresh();
                    }, function () {
                        e.archiveButton.state = "error";
                    });
                }
            },
            subButtons: [
                {
                    labelKey: "feedback_btnDelete",
                    handler: function () {
                        e.archiveButton.state = "busy";
                        $scope.delete(e, function () {
                            e.archiveButton.state = "success";
                            $scope.refresh();
                        }, function () {
                            e.archiveButton.state = "error";
                        });
                    }
                }
            ]
        };

    } 

    $scope.update = function (p) {

        $scope.loading = true;

        var params = {
            key: $scope.current.key
        };

        if (p) {
            params.page = p;
        } else if ($scope.pagination && $scope.pagination.page) {
            params.page = $scope.pagination.page;
        }

        if ($scope.sorting.field) params.sort = $scope.sorting.field;
        if ($scope.sorting.order) params.order = $scope.sorting.order;

        $http.get("/umbraco/backoffice/Skybrud/FeedbackAdmin/GetEntriesForSite", { params: params }).then(function (res) {

            $scope.entries = res.data.entries.data;

            updateBadge(res.data.entries);
            updatePagination(res.data.entries);
            updateSorting(res.data.entries);

            $scope.entries.forEach(function (e) {
                updateButtons(e);
            });

            $scope.loading = false;

        });

    };

    $scope.update();

});