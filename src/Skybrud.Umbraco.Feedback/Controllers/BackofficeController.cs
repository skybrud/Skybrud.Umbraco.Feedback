using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Skybrud.Umbraco.Feedback.Config;
using Skybrud.Umbraco.Feedback.Extensions;
using Skybrud.Umbraco.Feedback.Interfaces;
using Skybrud.Umbraco.Feedback.Model;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
﻿using System.Net.Http;

namespace Skybrud.Umbraco.Feedback.Controllers {

    [JsonOnlyConfiguration]
    [PluginController("Feedback")]
    public class BackendController : UmbracoAuthorizedApiController {

        public FeedbackConfig Config {
            get { return FeedbackConfig.Current; }
        }

        public NameValueCollection QueryString {
            get { return HttpContext.Current.Request.QueryString; }
        }

        [HttpGet]
        public object GetEntry(int entryId) {

            // TODO: Detect culture from query string
            CultureInfo culture = new CultureInfo("en-US");

            FeedbackEntry entry = FeedbackUtils.GetFromId(entryId);

            if (entry == null) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.NotFound, "Entry not found."));

            return JsonMetaResponse.GetSuccess(FeedbackEntryResult.GetFromEntry(entry, Umbraco, culture));

        }

        [HttpGet]
        public object GetStatuses() {
            return JsonMetaResponse.GetSuccess(FeedbackConfig.Current.Statuses);
        }

        [HttpGet]
        public object GetEntries(int siteId) {

            // TODO: Detect culture from query string
            CultureInfo culture = new CultureInfo("en-US");

            // TODO: Handle filtering in the database
            IEnumerable<FeedbackEntry> entries;
            if (QueryString["status"] == null) {
                entries = FeedbackUtils.GetAllForSite(siteId);
            } else {
                entries = FeedbackUtils.GetAllForSite(siteId).Where(x => x.Status.Alias == QueryString["status"]);
            }





            IEnumerable<FeedbackEntryResult> hai = entries.Select(x => FeedbackEntryResult.GetFromEntry(x, Umbraco, culture));

            string field = QueryString["sort"];
            string order = QueryString["order"] == "desc" ? "desc" : "asc";

            // TODO: Handle sorting in the database
            switch (field) {
                case "id":
                    hai = hai.OrderBy(x => x.Id, order == "desc");
                    break;
                case "pagename":
                    hai = hai.OrderBy(x => x.Page == null ? "" : x.Page.Name, order == "desc");
                    break;
                case "name":
                    hai = hai.OrderBy(x => x.Name, order == "desc");
                    break;
                case "rating":
                    hai = hai.OrderBy(x => x.Rating.Name, order == "desc");
                    break;
                case "status":
                    hai = hai.OrderBy(x => x.Status.Name, order == "desc");
                    break;
                case "created":
                    hai = hai.OrderBy(x => x.CreatedDateTime, order == "desc");
                    break;
                default:
                    field = "created";
                    hai = hai.OrderBy(x => x.CreatedDateTime, true);
                    break;
            }





            int limit;
            if (Int32.TryParse(QueryString["limit"], out limit)) {
                limit = Math.Max(1, Math.Min(100, limit));
            } else {
                limit = 15;
            }






            int total = hai.Count();
            int pages = (int) Math.Ceiling((double) total / limit);
            int page = Math.Max(1, Math.Min(pages, QueryString.GetInt32("page")));
            int offset = limit * (page - 1);

            FeedbackEntryResult[] data = hai.Skip(offset).Take(limit).ToArray();

            foreach (var result in data) {
                foreach (IFeedbackPlugin plugin in FeedbackModule.Instance.Plugins) {
                    try {
                        plugin.OnEntryResultRender(FeedbackModule.Instance, result);
                    } catch (Exception ex) {
                        LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnEntryResultRender.", ex);
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new {
                meta = new {
                    code = 200
                },
                pagination = new {
                    limit,
                    offset,
                    total,
                    page,
                    pages
                },
                sorting = new {
                    field,
                    order
                },
                data
            });

        }

        [HttpGet]
        public object GetFeedback(int siteId) {
            return GetEntries(siteId);
        }

        [HttpGet]
        public object GetUsers() {
            try {
                return JsonMetaResponse.GetSuccess(FeedbackModule.Instance.GetUsers().Values.OrderBy(x => x.Name));
            } catch (Exception) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, null));
            }
        }

        [HttpGet]
        public object SetResponsible(int entryId, int userId) {

            // TODO: Detect culture from query string
            CultureInfo culture = new CultureInfo("en-US");

            IFeedbackUser user = null;

            try {

                if (userId < 0) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Invalid user ID specified."));
                }

                if (userId > 0) {
                    user = FeedbackModule.Instance.GetUser(userId);
                    if (user == null) {
                        return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "A user with the specified ID could not be found."));
                    }
                }

                FeedbackEntry entry = FeedbackUtils.GetFromId(entryId);
                if (entry == null) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "An entry with the specified ID could not be found."));
                }

                if (entry.IsArchived) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "The entry has been archived and can therefore not be changed."));
                }

                if (!FeedbackModule.Instance.SetAssignedTo(entry, user)) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "The request was cancelled by the server."));
                }

                return JsonMetaResponse.GetSuccess(FeedbackEntryResult.GetFromEntry(entry, Umbraco, culture));

            } catch (Exception) {

                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, null));

            }

        }

        [HttpGet]
        public object SetStatus(int entryId, string alias) {

            // TODO: Detect culture from query string
            CultureInfo culture = new CultureInfo("en-US");

            try {

                FeedbackEntry entry = FeedbackUtils.GetFromId(entryId);
                if (entry == null) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "An entry with the specified ID could not be found."));
                }

                if (entry.IsArchived) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "The entry has been archived and can therefore not be changed."));
                }

                FeedbackStatus status = FeedbackConfig.Current.GetStatus(alias);
                if (status == null) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "A status with the specified alias could not be found."));
                }

                entry.ChangeStatus(status);

                return JsonMetaResponse.GetSuccess(FeedbackEntryResult.GetFromEntry(entry, Umbraco, culture));

            } catch (Exception) {
            
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, null));
            
            }

        }

        [HttpGet]
        public object Archive(int entryId) {

            // Set a danish culture
            CultureInfo culture = new CultureInfo("da");

            FeedbackEntry entry = FeedbackUtils.GetAll().FirstOrDefault(x => x.Id == entryId);
            if (entry == null) return JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Den angivne besvarelse blev ikke fundet.");

            entry.Archive();

            return JsonMetaResponse.GetSuccess(FeedbackEntryResult.GetFromEntry(entry, Umbraco, culture));

        }

    }

}