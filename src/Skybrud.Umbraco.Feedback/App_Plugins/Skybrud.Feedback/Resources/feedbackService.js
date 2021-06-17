angular.module("umbraco.resources").factory("feedbackService", function ($http) {
    return {
        archiveEntry: function (entryId) {
            return $http.get('/umbraco/backoffice/Feedback/Backend/ArchiveEntry?entryId=' + entryId);
        },
        archiveEntries: function (ids) {
            return $http.get("/umbraco/backoffice/Feedback/Backend/ArchiveEntries?ids=" + ids.join(","));
        },
        deleteEntry: function (entryId) {
            return $http.get('/umbraco/backoffice/Feedback/Backend/DeleteEntry?entryId=' + entryId);
        },
        deleteEntries: function (ids) {
            return $http.get("/umbraco/backoffice/Feedback/Backend/DeleteEntries?ids=" + ids.join(","));
        },
        getEntry: function (entryId) {
            return $http.get('/umbraco/backoffice/Feedback/Backend/GetEntry?entryId=' + entryId);
        },
        getEntries: function (siteId, page, limit, sort, order, filters) {

            var params = {
                siteId: siteId,
                page: page,
                limit: limit,
                sort: sort,
                order: order
            };

            if (filters) {
                if (filters.rating && filters.rating != 'all') params.rating = filters.rating;
                if (filters.responsible && filters.responsible != 'all') params.responsible = filters.responsible;
                if (filters.status && filters.status != 'all') params.status = filters.status;
                if (filters.type && filters.type != 'all') params.type = filters.type;
            }

            return $http({
                method: 'GET',
                url: '/umbraco/backoffice/Feedback/Backend/GetEntries',
                params: params
            });

        },
        getStatuses: function () {
            return $http.get('/umbraco/backoffice/Feedback/Backend/GetStatuses');
        },
        getUsers: function () {
            return $http.get('/umbraco/backoffice/Feedback/Backend/GetUsers');
        },
        setResponsible: function (entryId, userId) {
            return $http.get('/umbraco/backoffice/Feedback/Backend/SetResponsible?entryId=' + entryId + '&userId=' + userId);
        },
        setStatus: function (entryId, alias) {
            if (Array.isArray(entryId)) {
                return $http.get('/umbraco/backoffice/Feedback/Backend/BatchSetStatus?ids=' + entryId.join(",") + '&alias=' + alias);
            } else {
                return $http.get('/umbraco/backoffice/Feedback/Backend/SetStatus?entryId=' + entryId + '&alias=' + alias);
            }
        },
        getColumns: function () {
            return [
                { alias: 'pagename', name: 'Page' },
                { alias: 'responsible', name: 'Asssigned to' },
                { alias: 'name', name: 'Name' },
                { alias: 'comment', name: 'Comment' },
                { alias: 'rating', name: 'Rating' },
                { alias: 'status', name: 'Status' },
                { alias: 'added', name: 'Added' }
            ];
        },
        getFilters: function () {
            return [
                { alias: 'rating', name: 'Rating' },
                { alias: 'responsible', name: 'Responsible' },
                { alias: 'status', name: 'Status' },
                { alias: 'type', name: 'Type' }
            ];
        }
    };
});