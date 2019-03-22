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
using Skybrud.Umbraco.Feedback.Model.Entries;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
﻿using System.Net.Http;
using System.Text.RegularExpressions;
using Skybrud.Umbraco.Feedback.Exceptions;
using Skybrud.Umbraco.Feedback.Models;
using Umbraco.Web.Editors;

#pragma warning disable 1591

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
        public object GetStatusesForSite(int siteId) {
            return FeedbackModule.Instance.GetStatusesForSite(siteId);
        }

        [HttpGet]
        public object GetRatingsForSite(int siteId) {
            return FeedbackModule.Instance.GetRatingsForSite(siteId);
        }

        [HttpGet]
        public object GetEntries(int siteId) {

            // TODO: Detect culture from query string
            CultureInfo culture = new CultureInfo("en-US");

            // TODO: Handle filtering in the database
            IEnumerable<FeedbackEntry> entries = FeedbackUtils.GetAllForSite(siteId);

            // Filter the entries by rating
            if (QueryString["rating"] != null) {
                entries = entries.Where(x => x.Rating.Alias == QueryString["rating"]);
            }

            // Filter the entries by responsible
            if (QueryString["responsible"] != null) {
                int responsibleId;
                if (Int32.TryParse(QueryString["responsible"], out responsibleId)) {
                    entries = entries.Where(x => (responsibleId == -1 && x.AssignedTo == null) || (x.AssignedTo != null && x.AssignedTo.Id == responsibleId));
                }
            }

            // Filter the entries by status
            if (QueryString["status"] != null) {
                entries = entries.Where(x => x.Status.Alias == QueryString["status"]);
            }

            // Filter the entries by type
            if (QueryString["type"] == "rating") {
                entries = entries.Where(x => x.IsRatingOnly);
            } else if (QueryString["type"] == "comment") {
                entries = entries.Where(x => x.HasComment);
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
                return FeedbackModule.Instance.GetUsers().Values.OrderBy(x => x.Name);
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

                // A valid user ID is a positive number. "-1" is allowed as well since it indicates
                // that the entry should not be assigned to anyone
                if (userId < -1) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Invalid user ID specified."));

                // Further validate the specified user (if specified)
                if (userId >= 0) {
                    user = FeedbackModule.Instance.GetUser(userId);
                    if (user == null) return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "A user with the specified ID could not be found."));
                }
                
                // Look up the feedback entry
                FeedbackEntry entry = FeedbackUtils.GetFromId(entryId);
                if (entry == null) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "An entry with the specified ID could not be found."));
                }

                // If the entry is archived, we're not allowed to modify it
                if (entry.IsArchived) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "The entry has been archived and can therefore not be changed."));
                }

                // Set the assigned user (it may be cancelled by a plugin)
                if (!FeedbackModule.Instance.SetAssignedTo(entry, user)) {
                    return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "The request was cancelled by the server."));
                }

                // Return the entry result
                return FeedbackEntryResult.GetFromEntry(entry, Umbraco, culture);

            } catch (Exception) {

                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, null));

            }

        }

        [HttpGet]
        public object SetStatus(int entryId, string alias) {

            // TODO: Detect culture from query string
            CultureInfo culture = new CultureInfo("en-US");

            // Initialize a new service
            FeedbackService service = new FeedbackService();

            try {

                // Get a reference to the entry
                FeedbackEntry entry = service.GetEntryById(entryId);
                if (entry == null) throw new FeedbackHttpException("An entry with the specified ID could not be found.");

                // Has the entry been archived?
                if (entry.IsArchived) throw new FeedbackHttpException("The entry has been archived and can therefore not be changed.");

                // Get a reference to the specified status
                FeedbackStatus status = FeedbackConfig.Current.GetStatus(alias);
                if (status == null) throw new FeedbackHttpException("A status with the specified alias could not be found.");

                // Update the status
                service.SetStatus(entry, status);

                return JsonMetaResponse.GetSuccess(FeedbackEntryResult.GetFromEntry(entry, Umbraco, culture));

            } catch (FeedbackHttpException ex) {
            
                return Request.CreateResponse(JsonMetaResponse.GetError(ex.Code, ex.Message));
            
            } catch (Exception ex) {

                LogHelper.Error<BackOfficeController>("Failed setting status for entry with ID " + entryId + ".", ex);
            
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, null));
            
            }

        }

        [HttpGet]
        public object ArchiveEntry(int entryId) {

            // Set a danish culture
            CultureInfo culture = new CultureInfo("da");

            // Initialize a new service
            FeedbackService service = new FeedbackService();

            // Get a reference to the entry
            FeedbackEntry entry = service.GetEntryById(entryId);
            if (entry == null) return JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Den angivne besvarelse blev ikke fundet.");

            // Set the entry as archived
            service.Archive(entry);

            return JsonMetaResponse.GetSuccess(FeedbackEntryResult.GetFromEntry(entry, Umbraco, culture));

        }

        [HttpGet]
        [HttpDelete]
        public object DeleteEntry(int entryId) {

            // Set a danish culture
            CultureInfo culture = new CultureInfo("da");

            // Initialize a new service
            FeedbackService service = new FeedbackService();

            // Get a reference to the entry
            FeedbackEntry entry = service.GetEntryById(entryId);
            if (entry == null) return JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Den angivne besvarelse blev ikke fundet.");

            // Delete the entry
            service.Delete(entry);

            return JsonMetaResponse.GetSuccess(FeedbackEntryResult.GetFromEntry(entry, Umbraco, culture));

        }

        [HttpGet]
        [HttpDelete]
        public object DeleteAll(string date) {

            // Initialize a new service
            FeedbackService service = new FeedbackService();

            try {

                // Validate the date
                if (Regex.IsMatch(date, "^([0-9]{4}-[0-9]{2}-[0-9]{2})$") == false) throw new FeedbackHttpException("Invalid date specified.");

                // Parse the date
                DateTime dt = DateTime.Parse(date, CultureInfo.InvariantCulture);

                // Delete past entries in the database
                int count = service.DeleteAll(dt);

                return JsonMetaResponse.GetSuccess(new { date, count });

            } catch (FeedbackHttpException ex) {
            
                return Request.CreateResponse(JsonMetaResponse.GetError(ex.Code, ex.Message));
            
            } catch (Exception ex) {

                LogHelper.Error<BackOfficeController>("Failed deleting all entries before date " + date + ".", ex);
            
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, "Failed deleting all entries before date " + date + "."));
            
            }

        }

    }

}