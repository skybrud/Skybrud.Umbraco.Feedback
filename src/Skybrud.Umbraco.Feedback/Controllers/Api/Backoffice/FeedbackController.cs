using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Umbraco.Feedback.Models.Api;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Skybrud.Umbraco.Feedback.Models.Ratings;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Skybrud.Umbraco.Feedback.Models.Users;
using Skybrud.Umbraco.Feedback.Services;
using Skybrud.WebApi.Json;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
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

        public object GetEntriesForSite(Guid key, int page = 1, string sort = null, string order = null, string rating = null, string responsible = null, string status = null, string type = null) {

            CultureInfo culture = Security.CurrentUser.GetUserCulture(_localizedTextService, _globalSettings);

            if (_feedbackService.TryGetSite(key, out FeedbackSiteSettings site) == false) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

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


            if (Guid.TryParse(rating, out Guid ratingKey)) {
                options.Rating = ratingKey;
            }

            if (int.TryParse(responsible, out int responsibleId)) {
                options.Responsible = Current.Services.UserService.GetUserById(responsibleId)?.Key;
            } else if (Guid.TryParse(responsible, out Guid responsibleKey)) {
                options.Responsible = responsibleKey;
            }
            
            if (Guid.TryParse(status, out Guid statusKey)) {
                options.Status = statusKey;
            }

            options.Type = EnumUtils.ParseEnum(type, FeedbackEntryType.All);












            var result = _feedbackService.GetEntries(options);

            var siteModel = new SiteApiModel(site, _localizedTextService, culture);

            List<EntryApiModel> entries = new List<EntryApiModel>();

            Dictionary<Guid, PageApiModel> pages = new Dictionary<Guid, PageApiModel>();

            foreach (var entry in result.Entries) {

                if (!site.TryGetRating(entry.Dto.Rating, out var er)) {
                    er = new FeedbackRating(entry.Dto.Rating, "not-found");
                }

                if (!site.TryGetStatus(entry.Dto.Status, out var es)) {
                    es = new FeedbackStatus(entry.Dto.Status, "not-found");
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

                var r = new RatingApiModel(er, _localizedTextService, culture);
                var s = new StatusApiModel(es, _localizedTextService, culture);

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

        [HttpGet]
        public object GetUsers() {
            return _feedbackService.GetUsers();
        }

        [HttpPost]
        public object SetStatus([FromBody] JObject model) {

            CultureInfo culture = Security.CurrentUser.GetUserCulture(_localizedTextService, _globalSettings);

            Guid entryKey = model.GetGuid("entry");
            Guid statusKey = model.GetGuid("status");

            if (entryKey == Guid.Empty) return Request.CreateResponse(HttpStatusCode.BadRequest);
            if (statusKey == Guid.Empty) return Request.CreateResponse(HttpStatusCode.BadRequest);

            // Get the entry
            FeedbackEntry entry = _feedbackService.GetEntryByKey(entryKey);
            if (entry == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            // Get the site of the entry
            if (_feedbackService.TryGetSite(entry.SiteKey, out FeedbackSiteSettings site) == false) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }


            site.TryGetRating(entry.Dto.Rating, out FeedbackRating rating);

            // Get the status
            if (site.TryGetStatus(statusKey, out FeedbackStatus status) == false) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            _feedbackService.SetStatus(entry, status);

            var r = rating == null ? null : new RatingApiModel(rating, _localizedTextService, culture);
            var s = new StatusApiModel(entry.Status, _localizedTextService, culture);

            var siteModel = new SiteApiModel(site, _localizedTextService, culture);

            IFeedbackUser user = null;
            if (entry.Dto.AssignedTo != Guid.Empty) _feedbackService.TryGetUser(entry.Dto.AssignedTo, out user);

            return new EntryApiModel(entry, siteModel, TryGetPage(entry.PageKey, out var page) ? page : null, s, r, user);

        }

        [HttpPost]
        public object SetResponsible([FromBody] JObject model) {

            CultureInfo culture = Security.CurrentUser.GetUserCulture(_localizedTextService, _globalSettings);

            Guid entryKey = model.GetGuid("entry");
            Guid responsibleKey = model.GetGuid("responsible");

            if (entryKey == Guid.Empty) return Request.CreateResponse(HttpStatusCode.BadRequest);

            // Get the entry
            FeedbackEntry entry = _feedbackService.GetEntryByKey(entryKey);
            if (entry == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            // Get the site of the entry
            if (_feedbackService.TryGetSite(entry.SiteKey, out FeedbackSiteSettings site) == false) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            IFeedbackUser user = null;
            if (responsibleKey == Guid.Empty) {
                _feedbackService.SetAssignedTo(entry, null);
            } else {
                if (_feedbackService.TryGetUser(responsibleKey, out user) == false) {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
                _feedbackService.SetAssignedTo(entry, user);
            }

            var r = entry.Rating == null ? null : new RatingApiModel(entry.Rating, _localizedTextService, culture);
            var s = entry.Status == null ? null : new StatusApiModel(entry.Status, _localizedTextService, culture);

            var siteModel = new SiteApiModel(site, _localizedTextService, culture);

            return new EntryApiModel(entry, siteModel, TryGetPage(entry.PageKey, out var page) ? page : null, s, r, user);

        }

        private bool TryGetPage(Guid key, out PageApiModel result) {


            IPublishedContent publishedContent = _umbracoContextAccessor.UmbracoContext.Content.GetById(key);
            if (publishedContent != null) {
                result = new PageApiModel(publishedContent);
                return true;
            }
            
            IContent content = Services.ContentService.GetById(key);
            if (content != null) {
                result = new PageApiModel(content);
                return true;
            }

            result = null;
            return false;

        }

    }

}