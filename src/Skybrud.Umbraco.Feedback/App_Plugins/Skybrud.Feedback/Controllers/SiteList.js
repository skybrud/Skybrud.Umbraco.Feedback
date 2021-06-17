angular.module("umbraco").controller("Skybrud.Feedback.SiteListController", function ($scope, $http, $routeParams, userService, notificationsService, dialogService, feedbackService, localizationService) {

    // Get the ID of the current site
    var siteId = $routeParams.id;

    $scope.hasFilter = false;
    $scope.selectionCount = 0;

    $scope.statuses = [];




    // Get information about the current user
    userService.getCurrentUser().then(function (user) {
        $scope.user = user;
        init();
    });

    $scope.toggleExpanded = function (item, event) {
        if (event.target.localName === "input") return;
        if (event.target.localName === "td" && event.target.classList.contains("col-checkbox")) return;
        item.expanded = (item.expanded ? false : true);
    };

    $scope.onSelectionUpdate = function() {
        $scope.selectionCount = $scope.items.filter(x => x.selected).length;
    };


    $scope.setStatusForSelection = function(status) {

        const ids = $scope.items.filter(x => x.selected).map(x => x.id);
        if (ids.length == 0) return;

        feedbackService.setStatus(ids, status.alias).success(function (r) {
            notificationsService.success('The status was successfully updated.');
            $scope.updateList();
        }).error(function (r) {
            notificationsService.error('Unable to set status' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));
        });

    };

    $scope.archiveSelection = function () {

        const ids = $scope.items.filter(x => x.selected).map(x => x.id);
        if (ids.length == 0) return;

        if (!confirm("Are you sure you want to archive the selected feedback entries?")) return;

        feedbackService.archiveEntries(ids).success(function () {

            notificationsService.success("The entry was successfully archived.");

            $scope.updateList();

        }).error(function (r) {

            notificationsService.error("Unable to archive one or more entries" + (r && r.meta && r.meta.error ? ": " + r.meta.error : ""));

        });

    };

    $scope.deleteSelection = function (entry) {

        const ids = $scope.items.filter(x => x.selected).map(x => x.id);
        if (ids.length == 0) return;

        if (!confirm("Are you sure you want to delete the selected feedback entries?")) return;

        feedbackService.deleteEntries(ids).success(function () {

            notificationsService.success("The entries was successfully deleted.");

            $scope.updateList();

        }).error(function (r) {

            notificationsService.error("Unable to delete one or more entries" + (r && r.meta && r.meta.error ? ": " + r.meta.error : ""));

        });

    };

    $scope.setStatus = function (item, status) {
        if (!status) return;
        feedbackService.setStatus(item.id, status.alias).success(function (r) {
            item.status = r.data.status;
            notificationsService.success('The status was successfully updated.');
        }).error(function (r) {
            notificationsService.error('Unable to set status' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));
        });
    };











    function initFilters() {

        $scope.filters = {};

        $scope.filters.ratings = [
            { name: 'All ratings', alias: 'labelAllRatings', value: 'all' }
        ];

        $scope.filters.users = [
            { name: 'All users', alias: 'labelAllUsers', value: 'all' },
            { name: 'No responsible', alias: 'labelNoResponsible', value: '-1' },
            { name: 'Me', alias: 'labelMe', value: $scope.user.id + '' }
        ];

        $scope.filters.statuses = [
            { name: 'All statuses', alias: 'labelAllStatuses', value: 'all' }
        ];

        $scope.filters.types = [
            { name: 'All types', alias: 'labelAllTypes', value: 'all' },
            { name: 'Only with rating', alias: 'labelOnlyWithRating', value: 'rating' },
            { name: 'Rating and comment', alias: 'labelRatingAndComment', value: 'comment' }
        ];





        angular.forEach($scope.filters.ratings, function (e) {
            e.name = localizationService.localize('feedback_' + e.alias);
        });

        angular.forEach($scope.filters.users, function (e) {
            e.name = localizationService.localize('feedback_' + e.alias);
        });

        angular.forEach($scope.filters.statuses, function (e) {
            e.name = localizationService.localize('feedback_' + e.alias);
        });

        angular.forEach($scope.filters.types, function (e) {
            e.name = localizationService.localize('feedback_' + e.alias);
        });






        $http.get('/umbraco/backoffice/Feedback/Backend/GetRatingsForSite?siteId=' + siteId).success(function (r) {
            angular.forEach(r, function (rating) {
                $scope.filters.ratings.push({ name: rating.name, value: rating.alias });
            });
        });

        $http.get('/umbraco/backoffice/Feedback/Backend/GetUsers').success(function (r) {
            angular.forEach(r, function (user) {
                if (user.id == $scope.user.id) return;
                $scope.filters.users.push({ name: user.name, value: user.id + '' });
            });
        });

        $http.get('/umbraco/backoffice/Feedback/Backend/GetStatusesForSite?siteId=' + siteId).success(function (r) {
            angular.forEach(r, function (status) {
                $scope.statuses = r;
                $scope.filters.statuses.push({ name: status.name, value: status.alias });
            });
        });

        // Set the selection (first choice of each filter)
        $scope.selected = {};
        $scope.selected.rating = $scope.filters.ratings[0];
        $scope.selected.responsible = $scope.filters.users[0];
        $scope.selected.status = $scope.filters.statuses[0];
        $scope.selected.type = $scope.filters.types[0];

    }

    function init() {

        var sorting = {
            field: 'created',
            order: 'desc'
        };

        var pagination = $scope.pagination = {
            page: 1,
            pages: 0,
            limit: $scope.model.config.limit ? $scope.model.config.limit : 10,
            offset: 0,
            total: 0,
            pagination: []
        };


        initFilters();















        $scope.loading = true;

        $scope.response = null;

        $scope.items = [];

        $scope.sort = function (field, order) {

            if (order != 'desc') order = 'asc';

            if (field == sorting.field) {
                sorting.order = (sorting.order == 'asc' ? 'desc' : 'asc');
            } else {
                sorting.field = field;
                sorting.order = order;
            }

            $scope.updateList();

        };

        $scope.isSortDirection = function (field, order) {
            return field == sorting.field && order == sorting.order;
        };

        $scope.prev = function () {
            if (pagination.page > 1) $scope.updateList(pagination.page - 1);
        };

        $scope.next = function () {
            if (pagination.page < pagination.pages) $scope.updateList(pagination.page + 1);
        };

        $scope.updateList = function (page, limit) {

            $scope.loading = true;

            if (!page) page = $scope.pagination.page;
            if (!limit) limit = $scope.pagination.limit;

            var filters = {
                rating: $scope.selected.rating.value,
                responsible: $scope.selected.responsible.value,
                status: $scope.selected.status.value,
                type: $scope.selected.type.value
            };

            feedbackService.getEntries(siteId, page, limit, sorting.field, sorting.order, filters).success(function (r) {

                $scope.loading = false;

                angular.forEach(r.data, function (item) {
                    item.$comment = [];
                    angular.forEach(item.comment, function (line) {
                        item.$comment.push({ text: line });
                    });
                });

                $scope.items = r.data;
                $scope.selectionCount = 0;

                pagination.page = r.pagination.page;
                pagination.pages = r.pagination.pages;
                pagination.limit = r.pagination.limit;
                pagination.offset = r.pagination.offset;
                pagination.total = r.pagination.total;

                sorting = r.sorting;

                pagination.pagination = [];

                for (var i = 0; i < pagination.pages; i++) {
                    pagination.pagination.push({
                        val: (i + 1),
                        isActive: pagination.page == (i + 1)
                    });
                }

                $scope.response = r;

            }).error(function (r) {

                $scope.loading = false;

                notificationsService.error('Unable to load list' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));

            });

        };


        $scope.archive = function (entry) {

            if (!confirm('Are you sure you want to archive the selected feedback entry?')) return;

            feedbackService.archiveEntry(entry.id).success(function () {

                notificationsService.success('The entry was successfully archived.');

                $scope.updateList();

            }).error(function (r) {

                notificationsService.error('Unable to archive entry' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));

            });

        };

        $scope.delete = function (entry) {

            if (!confirm('Are you sure you want to delete the selected feedback entry?')) return;

            feedbackService.deleteEntry(entry.id).success(function () {

                notificationsService.success('The entry was successfully deleted.');

                $scope.updateList();

            }).error(function (r) {

                notificationsService.error('Unable to delete entry' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));

            });

        };

        $scope.openChangeStatus = function (item) {
            dialogService.closeAll();
            dialogService.open({
                template: '/App_Plugins/Skybrud.Feedback/Views/ChangeStatus.html',
                modalClass: 'umb-modal feedback-modal',
                dialogData: item,
                callback: function (status) {
                    if (!status) return;
                    feedbackService.setStatus(item.id, status.alias).success(function (r) {
                        item.status = r.data.status;
                        notificationsService.success('The status was successfully updated.');
                    }).error(function (r) {
                        notificationsService.error('Unable to set status' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));
                    });
                }
            });
        };

        $scope.openSelectResponsible = function (item) {
            dialogService.closeAll();
            dialogService.open({
                template: '/App_Plugins/Skybrud.Feedback/Views/SelectResponsible.html',
                modalClass: 'umb-modal feedback-modal',
                dialogData: item,
                callback: function (user) {
                    feedbackService.setResponsible(item.id, user ? user.id : -1).success(function (r) {
                        item.assignedTo = r.assignedTo;
                        notificationsService.success('The responsible was successfully updated.');
                    }).error(function (r) {
                        notificationsService.error('Unable to set responsible' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));
                    });
                }
            });
        };

        // Refreshes the list
        $scope.refresh = function () {
            $scope.updateList();
        };


        $scope.deleteAll = function () {
            $scope.overlay = {
                title: "Oprydning",
                view: "/App_Plugins/Skybrud.Feedback/Views/Overlays/DeleteAll.html",
                show: true,
                submitButtonLabel: "Fortsæt",
                submit: function (model) {
                    $http.get("/umbraco/backoffice/Feedback/Backend/DeleteAll?date=" + model.date).success(function(res) {
                        $scope.overlay.show = false;
                        $scope.overlay = null;
                        notificationsService.success("Oprydning", "Der blev slettet " + res.data.count + " besvarelser.");
                        if (res.data.count) $scope.refresh();
                    }).error(function() {
                        notificationsService.error("Oprydning", "Der skete en fejl på serveren. Prøv venligst igen.");
                    });
                }
            };
        };

        // Add a watcher on the selection
        $scope.$watch('selected', function () {
            $scope.hasFilter = (
                $scope.selected.rating != 'all'
                ||
                $scope.selected.responsible != 'all'
                ||
                $scope.selected.status != 'all'
                ||
                $scope.selected.type != 'all'
            );
            $scope.updateList();
        }, true);

    }

});