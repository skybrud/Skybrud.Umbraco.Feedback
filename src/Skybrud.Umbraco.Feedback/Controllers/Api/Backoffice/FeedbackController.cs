using Microsoft.AspNetCore.Mvc;
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
using System;
using System.Collections.Generic;
using System.Globalization;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Controllers.Api.Backoffice {

    [PluginController("Skybrud")]
    public class FeedbackAdminController : UmbracoAuthorizedApiController {

        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly FeedbackService _feedbackService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IContentService _contentService;
        private readonly IUserService _userService;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

        #region Constructors

        public FeedbackAdminController(IUmbracoContextAccessor umbracoContextAccessor, FeedbackService feedbackService, ILocalizedTextService localizedTextService, IContentService contentService, IUserService userService, IBackOfficeSecurityAccessor backOfficeSecurityAccessor) {
            _umbracoContextAccessor = umbracoContextAccessor;
            _feedbackService = feedbackService;
            _localizedTextService = localizedTextService;
            _contentService = contentService;
            _userService = userService;
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        }

        #endregion

        #region Public API methods

        [HttpGet]
        public object Archive(Guid key) {

            var entry = _feedbackService.GetEntryByKey(key);
            if (entry == null) {
                return NotFound();
            }

            _feedbackService.Archive(entry);

            return entry;

        }

        [HttpGet]
        public object Delete(Guid key) {

            var entry = _feedbackService.GetEntryByKey(key);
            if (entry == null) {
                return NotFound();
            }

            _feedbackService.Delete(entry);

            return entry;

        }

        public object GetEntriesForSite(Guid key, int page = 1, string sort = null, string order = null, string rating = null, string responsible = null, string status = null, string type = null) {

            CultureInfo culture = new CultureInfo(_backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser.Language);

            if (_feedbackService.TryGetSite(key, out FeedbackSiteSettings site) == false) {
                return NotFound();
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
                options.Responsible = _userService.GetUserById(responsibleId)?.Key;
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

                if (!pages.TryGetValue(entry.PageKey, out PageApiModel pageModel) && _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext)) {
                    var c1 = umbracoContext.Content.GetById(entry.PageKey);
                    if (c1 != null) {
                        pages.Add(entry.PageKey, pageModel = new PageApiModel(c1));
                    } else {
                        var c2 = _contentService.GetById(entry.PageKey);
                        if (c2 != null) {
                            pages.Add(entry.PageKey, pageModel = new PageApiModel(c2));

                        }
                    }
                }

                IFeedbackUser user = null;
                if (entry.Dto.AssignedTo != Guid.Empty) {
                    _feedbackService.TryGetUser(entry.Dto.AssignedTo, out user);
                }

                var r = new RatingApiModel(er, _localizedTextService, culture);
                var s = new StatusApiModel(es, _localizedTextService, culture);

                entries.Add(new EntryApiModel(entry, siteModel, pageModel, s, r, user));

            }

            return new {
                site = siteModel,
                entries = new {
                    pagination = new {
                        page,
                        pages = (int) Math.Ceiling(result.Total / (double) result.PerPage),
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

            CultureInfo culture = new CultureInfo(_backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser.Language);

            Guid entryKey = model.GetGuid("entry");
            Guid statusKey = model.GetGuid("status");

            if (entryKey == Guid.Empty) {
                return BadRequest();
            }

            if (statusKey == Guid.Empty) {
                return BadRequest();
            }

            // Get the entry
            FeedbackEntry entry = _feedbackService.GetEntryByKey(entryKey);
            if (entry == null) {
                return NotFound();
            }

            // Get the site of the entry
            if (_feedbackService.TryGetSite(entry.SiteKey, out FeedbackSiteSettings site) == false) {
                throw new Exception();
            }


            site.TryGetRating(entry.Dto.Rating, out FeedbackRating rating);

            // Get the status
            if (site.TryGetStatus(statusKey, out FeedbackStatus status) == false) {
                throw new Exception();
            }

            _feedbackService.SetStatus(entry, status);

            var r = rating == null ? null : new RatingApiModel(rating, _localizedTextService, culture);
            var s = new StatusApiModel(entry.Status, _localizedTextService, culture);

            var siteModel = new SiteApiModel(site, _localizedTextService, culture);

            IFeedbackUser user = null;
            if (entry.Dto.AssignedTo != Guid.Empty) {
                _feedbackService.TryGetUser(entry.Dto.AssignedTo, out user);
            }

            return new EntryApiModel(entry, siteModel, TryGetPage(entry.PageKey, out var page) ? page : null, s, r, user);

        }

        [HttpPost]
        public object SetResponsible([FromBody] JObject model) {

            CultureInfo culture = new CultureInfo(_backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser.Language);

            Guid entryKey = model.GetGuid("entry");
            Guid responsibleKey = model.GetGuid("responsible");

            if (entryKey == Guid.Empty) {
                return BadRequest();
            }

            // Get the entry
            FeedbackEntry entry = _feedbackService.GetEntryByKey(entryKey);
            if (entry == null) {
                return NotFound();
            }

            // Get the site of the entry
            if (_feedbackService.TryGetSite(entry.SiteKey, out FeedbackSiteSettings site) == false) {
                throw new Exception();
            }

            IFeedbackUser user = null;
            if (responsibleKey == Guid.Empty) {
                _feedbackService.SetAssignedTo(entry, null);
            } else {
                if (_feedbackService.TryGetUser(responsibleKey, out user) == false) {
                    throw new Exception();
                }
                _feedbackService.SetAssignedTo(entry, user);
            }

            var r = entry.Rating == null ? null : new RatingApiModel(entry.Rating, _localizedTextService, culture);
            var s = entry.Status == null ? null : new StatusApiModel(entry.Status, _localizedTextService, culture);

            var siteModel = new SiteApiModel(site, _localizedTextService, culture);

            return new EntryApiModel(entry, siteModel, TryGetPage(entry.PageKey, out var page) ? page : null, s, r, user);

        }

        #endregion

        #region Private helper methods

        private bool TryGetPage(Guid key, out PageApiModel result) {

            _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext);
            IPublishedContent publishedContent = umbracoContext.Content.GetById(key);
            if (publishedContent != null) {
                result = new PageApiModel(publishedContent);
                return true;
            }

            IContent content = _contentService.GetById(key);
            if (content != null) {
                result = new PageApiModel(content);
                return true;
            }

            result = null;
            return false;

        }

        #endregion

    }

}