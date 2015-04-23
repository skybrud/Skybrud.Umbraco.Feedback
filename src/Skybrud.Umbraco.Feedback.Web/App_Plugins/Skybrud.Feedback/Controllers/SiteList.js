angular.module("umbraco").controller("Skybrud.Feedback.SiteListController", function ($scope, $http, $routeParams, notificationsService, dialogService, feedbackService) {

    var siteId = $routeParams.id;

    var sorting = {
        field: 'created',
        order: 'desc'
    };

    var pagination = $scope.pagination = {
        page: 1,
        pages: 0,
        limit: $scope.model.config.limit ? $scope.model.config.limit : 10,
        offset: 0,
        pagination: []
    };

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

    $scope.isSortDirection = function(field, order) {
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

        feedbackService.getEntries(siteId, page, limit, sorting.field, sorting.order).success(function (r) {

            $scope.loading = false;

            $scope.items = r.data;

            pagination.page = r.pagination.page;
            pagination.pages = r.pagination.pages;
            pagination.limit = r.pagination.limit;
            pagination.offset = r.pagination.offset;

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


    $scope.archive = function (entryId) {

        if (!confirm('Are you sure you want to archive the selected feedback entry?')) return;

        feedbackService.archiveEntry(entryId).success(function () {

            notificationsService.success('The entry was successfully archived.');

            $scope.updateList();

        }).error(function (r) {

            notificationsService.error('Unable to archive entry' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));

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
                feedbackService.setResponsible(item.id, user ? user.id : 0).success(function (r) {
                    item.assignedTo = r.data.assignedTo;
                    notificationsService.success('The responsible was successfully updated.');
                }).error(function (r) {
                    notificationsService.error('Unable to set responsible' + (r && r.meta && r.meta.error ? ': ' + r.meta.error : ''));
                });
            }
        });
    };

    $scope.updateList();

});