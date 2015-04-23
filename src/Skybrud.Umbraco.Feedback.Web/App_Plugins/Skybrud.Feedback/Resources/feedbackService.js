angular.module("umbraco.resources").factory("feedbackService", function ($http) {
    return {
        archiveEntry: function (entryId) {
            return $http.get('/umbraco/backoffice/Feedback/Backend/Archive?entryId=' + entryId);
        },
        getEntry: function (entryId) {
            return $http.get('/umbraco/backoffice/Feedback/Backend/GetEntry?entryId=' + entryId);
        },
        getEntries: function (siteId, page, limit, sort, order) {
            return $http({
                method: 'GET',
                url: '/umbraco/backoffice/Feedback/Backend/GetEntries',
                params: {
                    siteId: siteId,
                    page: page,
                    limit: limit,
                    sort: sort,
                    order: order
                }
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
            return $http.get('/umbraco/backoffice/Feedback/Backend/SetStatus?entryId=' + entryId + '&alias=' + alias);
        }
    };
});