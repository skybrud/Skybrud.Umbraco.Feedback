using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Skybrud.Umbraco.Feedback.Config;
using Skybrud.Umbraco.Feedback.Constants;
using Skybrud.Umbraco.Feedback.Exceptions;
using Skybrud.Umbraco.Feedback.Extensions;
using Skybrud.Umbraco.Feedback.Model;
using Skybrud.Umbraco.Feedback.Model.Entries;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Feedback.Controllers {

    [JsonOnlyConfiguration]
    [PluginController("Feedback")]
    public class FrontendController : UmbracoApiController {

        public const string ErrorInvalidConfig = "UTFMMH";
        public const string ErrorSiteNotFound = "DFG56G";
        public const string ErrorPageNotFound = "YAH2RQ";
        public const string ErrorRatingNotFound = "EDKGLD";
        public const string ErrorProfileNotFound = "54C39E";
        public const string ErrorCustom = "DFG5BF";
        public const string ErrorUnknown = "6GF36E";
        public const string ErrorRequired = "41AD86";
        public const string ErrorCancelled = "B02C58";

        public FeedbackModule Feedback {
            get { return FeedbackModule.Instance; }
        }

        public NameValueCollection FormData {
            get { return HttpContext.Current.Request.Form; }
        }

        private HttpResponseMessage GetError(object errorCode, string message) {
            return Request.CreateResponse(HttpStatusCode.InternalServerError, new {
                meta = new {
                    code = 500,
                    errorCode,
                    error = message
                },
                data = default(object)
            });
        }

        [HttpGet]
        public object GetConfig() {

            return JsonMetaResponse.GetSuccess(FeedbackConfig.Current);

        }

        [HttpPost]
        public object AddRating() {

            // Load the config file
            FeedbackConfig config;
            try {
                config = FeedbackConfig.Current;
            } catch (Exception) {
                return GetError(ErrorInvalidConfig, FeedbackConstants.ErrorMessages.DefaultSubmitError);
            }

            // Read from the form DATA
            int siteId = FormData.GetInt32("siteId");
            int pageId = FormData.ContainsKey("nodeId") ? FormData.GetInt32("nodeId") : FormData.GetInt32("pageId");
            string rating = FormData["rating"];

            // Get the page
            IPublishedContent site = Umbraco.TypedContent(siteId);
            IPublishedContent page = Umbraco.TypedContent(pageId);

            // Check whether the root node was found
            if (site == null) {
                return GetError(ErrorSiteNotFound, "A site with the specified ID could not be found.");
            }

            // Check whether the page was found
            if (page == null) {
                return GetError(ErrorPageNotFound, "A page with the specified ID could not be found.");
            }

            // Look for a profile either for the specified site or globally for "0"
            FeedbackProfile profile;
            if (!config.Profiles.TryGetValue(site.Id, out profile)) {
                if (!config.Profiles.TryGetValue(0, out profile)) {
                    return GetError(ErrorProfileNotFound, FeedbackConstants.ErrorMessages.DefaultSubmitError);
                }
            }

            // Validate the rating
            FeedbackRating rr = profile.GetRating(rating);
            if (rr == null) {
                return GetError(ErrorRatingNotFound, "A rating with the specified name does not exist.");
            }

            try {
                FeedbackEntry entry = FeedbackModule.Instance.AddRating(site, page, profile, rr);
                if (entry == null) return GetError(ErrorCancelled, "Your rating submission was cancelled by the server.");
                return new {
                    id = entry.Id,
                    uniqueId = entry.UniqueId,
                    rating = entry.Rating
                };
            } catch (FeedbackException ex) {
                return GetError(ex.Code, String.IsNullOrWhiteSpace(ex.Message) ? FeedbackConstants.ErrorMessages.DefaultSubmitError : ex.Message);
            } catch (Exception ex) {
                LogHelper.Error<FrontendController>("Error code: " + ErrorUnknown, ex);
                return GetError(ErrorUnknown, FeedbackConstants.ErrorMessages.DefaultSubmitError);
            }

        }

        [HttpPost]
        public object UpdateEntry() {

            try {
                
                // Load the config file
                FeedbackConfig config;
                try {
                    config = FeedbackConfig.Current;
                } catch (Exception) {
                    return GetError(ErrorInvalidConfig, "Your feedback could not be submitted due to an error on the server.");
                }

                // Read from the form DATA
                string uniqueId = FormData["uniqueId"];
                int siteId = FormData.GetInt32("siteId");
                int pageId = FormData.ContainsKey("nodeId") ? FormData.GetInt32("nodeId") : FormData.GetInt32("pageId");
                string name = FormData["name"];
                string email = FormData["email"];
                string comment = FormData["comment"];

                //// Get the page
                //IPublishedContent site = Umbraco.TypedContent(siteId);
                //IPublishedContent page = Umbraco.TypedContent(pageId);
            
                //// Check whether the root node was found
                //if (site == null) {
                //    return GetError(ErrorSiteNotFound, "A site with the specified ID could not be found.");
                //}

                //// Check whether the page was found
                //if (page == null) {
                //    return GetError(ErrorPageNotFound, "A page with the specified ID could not be found.");
                //}

                // Look for a profile either for the specified site or globally for "0"
                FeedbackProfile profile;
                if (!config.Profiles.TryGetValue(siteId, out profile)) {
                    if (!config.Profiles.TryGetValue(0, out profile)) {
                        return GetError(ErrorProfileNotFound, "Your feedback could not be submitted due to an error on the server.");
                    }
                }

                FeedbackEntry entry = Feedback.Entries.GetEntryById(uniqueId);
                if (entry == null) throw new FeedbackException("b96aa020-f0e7-4d41-baaa-58cb75b3a87d", "Your feedback could not be submitted due to an error on the server.");

                if (entry.SiteId != siteId) throw new FeedbackException("167fecbb-667f-42f1-af06-5d7d0eeee4e4", "Your feedback could not be submitted due to an error on the server.");
                if (entry.PageId != pageId) throw new FeedbackException("4f7a734c-e664-4bb6-9031-1990276bf5e2", "Your feedback could not be submitted due to an error on the server.");
            
                // Update the properties of the feedback entry
                entry.Name = name;
                entry.Email = email;
                entry.Comment = String.IsNullOrWhiteSpace(comment) ? null : comment.Trim();

                // If an editor has archived the entry, we restore it since it has changes
                entry.IsArchived = false;

                // Set the "Updated" timestamp
                entry.Updated = DateTime.UtcNow;

                // Update the feedback entry in the database
                Feedback.Entries.Save(entry);

                return new {
                    id = entry.Id,
                    uniqueId = entry.UniqueId,
                    rating = entry.Rating
                };

            } catch (FeedbackException ex) {
                return GetError(ex.Code, String.IsNullOrWhiteSpace(ex.Message) ? "Your feedback could not be submitted due to an error on the server." : ex.Message);
            } catch (Exception ex) {
                LogHelper.Error<FrontendController>("Error code: " + ErrorUnknown, ex);
                return GetError(ErrorUnknown, "Your feedback could not be submitted due to an error on the server.");
            }

        }

        public object PostFeedback() {

            // Load the config file
            FeedbackConfig config;
            try {
                config = FeedbackConfig.Current;
            } catch (Exception) {
                return GetError(ErrorInvalidConfig, "Your feedback could not be submitted due to an error on the server.");
            }

            // Read from the form DATA
            int siteId = FormData.GetInt32("siteId");
            int pageId = FormData.ContainsKey("nodeId") ? FormData.GetInt32("nodeId") : FormData.GetInt32("pageId");
            string name = FormData["name"];
            string email = FormData["email"];
            string rating = FormData["rating"];
            string comment = FormData["comment"];

            // Get the page
            IPublishedContent site = Umbraco.TypedContent(siteId);
            IPublishedContent page = Umbraco.TypedContent(pageId);

            // Check whether the root node was found
            if (site == null) {
                return GetError(ErrorSiteNotFound, "A site with the specified ID could not be found.");
            }

            // Check whether the page was found
            if (page == null) {
                return GetError(ErrorPageNotFound, "A page with the specified ID could not be found.");
            }

            // Look for a profile either for the specified site or globally for "0"
            FeedbackProfile profile;
            if (!config.Profiles.TryGetValue(site.Id, out profile)) {
                if (!config.Profiles.TryGetValue(0, out profile)) {
                    return GetError(ErrorProfileNotFound, "Your feedback could not be submitted due to an error on the server.");
                }
            }

            // Validate the rating
            FeedbackRating rr = profile.GetRating(rating);
            if (rr == null) {
                return GetError(ErrorRatingNotFound, "A rating with the specified name does not exist.");
            }

            // Validate the text fields
            if (profile.Fields.Email == FeedbackFieldType.Required && String.IsNullOrWhiteSpace(email)) return GetError(ErrorRequired, "One or more required fields must be specified.");
            if (profile.Fields.Name == FeedbackFieldType.Required && String.IsNullOrWhiteSpace(name)) return GetError(ErrorRequired, "One or more required fields must be specified.");
            if (profile.Fields.Comment == FeedbackFieldType.Required && String.IsNullOrWhiteSpace(comment)) return GetError(ErrorRequired, "One or more required fields must be specified.");

            FeedbackStatus status = config.GetStatus("new");

            try {
                FeedbackEntry entry = FeedbackModule.Instance.AddFeedbackComment(site, page, profile, rr, status, name, email, comment);
                if (entry == null) return GetError(ErrorCancelled, "Your feedback submission was cancelled by the server.");
                return JsonMetaResponse.GetSuccess(entry.Id);
            } catch (FeedbackException ex) {
                return GetError(ErrorCustom, ex.Message);
            } catch (Exception) {
                return GetError(ErrorUnknown, "Your feedback could not be submitted due to an error on the server.");
            }

        }

    }

}