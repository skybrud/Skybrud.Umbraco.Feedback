using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Skybrud.Essentials.Enums;
using Skybrud.Umbraco.Feedback.Models.Api;
using Skybrud.Umbraco.Feedback.Models.Ratings;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Skybrud.Umbraco.Feedback.Models.Users;
using Skybrud.Umbraco.Feedback.Services;
using Skybrud.WebApi.Json;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Feedback.Controllers.Api.Backoffice {

    [JsonOnlyConfiguration]
    [PluginController("Skybrud")]
    public class FeedbackAdminController : UmbracoAuthorizedApiController {

        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly FeedbackService _feedbackService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IGlobalSettings _globalSettings;

        public FeedbackAdminController(IUmbracoContextAccessor umbracoContextAccessor, FeedbackService feedbackService, ILocalizedTextService localizedTextService, IGlobalSettings globalSettings) {
            _umbracoContextAccessor = umbracoContextAccessor;
            _feedbackService = feedbackService;
            _localizedTextService = localizedTextService;
            _globalSettings = globalSettings;
        }

        [HttpGet]
        public object Archive(Guid key) {

            var entry = _feedbackService.GetEntryByKey(key);
            if (entry == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            _feedbackService.Archive(entry);

            return entry;

        }

        [HttpGet]
        public object Delete(Guid key) {

            var entry = _feedbackService.GetEntryByKey(key);
            if (entry == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            _feedbackService.Delete(entry);

            return entry;

        }

        public object GetEntriesForSite(Guid key, int page = 1, string sort = null, string order = null) {

            CultureInfo culture = Security.CurrentUser.GetUserCulture(_localizedTextService, _globalSettings);


            if (_feedbackService.TryGetSite(key, out FeedbackSiteSettings site) == false) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var request = new HttpRequestWrapper(HttpContext.Current.Request);

            FeedbackGetEntriesOptions options = new FeedbackGetEntriesOptions {
                Page = page,
                PerPage = 10,
                SiteKey = key
            };

            switch (sort) {
                
                case "rating":
                    options.SortField = EntriesSortField.Rating;
                    options.SortOrder = EnumUtils.ParseEnum(order, EntriesSortOrder.Asc);
                    break;

                case "status":
                    options.SortField = EntriesSortField.Status;
                    options.SortOrder = EnumUtils.ParseEnum(order, EntriesSortOrder.Asc);
                    break;

                default:
                    options.SortField = EntriesSortField.CreateDate;
                    options.SortOrder = EnumUtils.ParseEnum(order, EntriesSortOrder.Desc);
                    break;


            }

            var result = _feedbackService.GetEntries(options);

            var siteModel = new SiteApiModel(site, request, _localizedTextService, culture);

            List<EntryApiModel> entries = new List<EntryApiModel>();

            Dictionary<Guid, PageApiModel> pages = new Dictionary<Guid, PageApiModel>();

            foreach (var entry in result.Entries) {

                if (!site.TryGetRating(entry.Dto.Rating, out var rating)) {
                    rating = new FeedbackRating(entry.Dto.Rating, "not-found");
                }

                if (!site.TryGetStatus(entry.Dto.Status, out var status)) {
                    status = new FeedbackStatus(entry.Dto.Status, "not-found");
                }

                if (!pages.TryGetValue(entry.PageKey, out PageApiModel pageModel)) {
                    var c1 = _umbracoContextAccessor.UmbracoContext.Content.GetById(entry.PageKey);
                    if (c1 != null) {
                        pages.Add(entry.PageKey, pageModel = new PageApiModel(c1));
                    } else {
                        var c2 = Current.Services.ContentService.GetById(entry.PageKey);
                        if (c2 != null) {
                            pages.Add(entry.PageKey, pageModel = new PageApiModel(c2));
                            
                        }
                    }
                }

                IFeedbackUser user = null;
                if (entry.Dto.AssignedTo != Guid.Empty) _feedbackService.TryGetUser(entry.Dto.AssignedTo, out user);

                var r = new RatingApiModel(rating, request, _localizedTextService, culture);
                var s = new StatusApiModel(status, request, _localizedTextService, culture);

                entries.Add(new EntryApiModel(entry, siteModel, pageModel, s, r, user));

            }

            return new {
                site = siteModel,
                entries = new {
                    pagination = new {
                        page,
                        pages = (int) Math.Ceiling(result.Total / (double)result.PerPage),
                        limit = result.PerPage,
                        total = result.Total,
                        offset = (result.Page - 1) * result.PerPage,
                    },
                    sorting = new { field = options.SortField, order = options.SortOrder },
                    data = entries
                }
            };

        }
        
    }

}