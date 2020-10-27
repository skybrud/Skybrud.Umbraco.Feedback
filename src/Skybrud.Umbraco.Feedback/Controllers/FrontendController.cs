using System.Net;
using System.Net.Http;
using System.Web.Http;
using Skybrud.Umbraco.Feedback.Model;
using Skybrud.Umbraco.Feedback.Model.Entries;
using Skybrud.Umbraco.Feedback.Models;
using Skybrud.Umbraco.Feedback.Models.Api;
using Skybrud.Umbraco.Feedback.Models.Results;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Plugins;
using Skybrud.Umbraco.Feedback.Services;
using Skybrud.WebApi.Json;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Feedback.Controllers {

    [JsonOnlyConfiguration]
    [PluginController("Feedback")]
    public class FrontendController : UmbracoApiController {
        
        private readonly FeedbackService _feedbackService;
        
        private readonly FeedbackPluginCollection _feedbackPluginCollection;

        #region Constructors

        public FrontendController(FeedbackService feedbackService, FeedbackPluginCollection feedbackPluginCollection) {
            _feedbackService = feedbackService;
            _feedbackPluginCollection = feedbackPluginCollection;
        }

        #endregion

        #region Member methods

        [HttpPost]
        public object AddRating([FromBody] AddRatingModel model) {

            // Get site site
            if (!_feedbackPluginCollection.TryGetSite(model.SiteKey, out FeedbackSiteSettings site)) {
                return Request.CreateResponse(HttpStatusCode.NotFound, "A site with the specified key could not be found.");
            }
            
            // Get the page
            IPublishedContent page = Umbraco.Content(model.PageKey);
            if (page == null) return Request.CreateResponse(HttpStatusCode.NotFound, "A page with the specified key could not be found.");

            // Get the rating
            if (!site.TryGetRating(model.Rating, out FeedbackRating rating)) {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "A rating with the specified name does not exist.");
            }
            
            // Attempt to add the rating
            AddRatingResult result = _feedbackService.AddRating(site, page, rating);

            // Return a response matching the result
            switch (result.Status) {

                case AddRatingStatus.Success:
                    return new { key = result.Entry.Key };

                case AddRatingStatus.Cancelled:
                    return Request.CreateResponse(HttpStatusCode.BadRequest, result.Message);

                default:
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, result.Message);

            }

        }

        [HttpPost]
        public object UpdateEntry([FromBody] UpdateEntryModel model) {

            // Get site site
            if (!_feedbackPluginCollection.TryGetSite(model.SiteKey, out _)) {
                return Request.CreateResponse(HttpStatusCode.NotFound, "A site with the specified key could not be found.");
            }
            
            // Get the page
            IPublishedContent page = Umbraco.Content(model.PageKey);
            if (page == null) return Request.CreateResponse(HttpStatusCode.NotFound, "A page with the specified key could not be found.");

            // Get a reference to the entry
            FeedbackEntry entry =_feedbackService.GetEntryByKey(model.Key);
            if (entry == null) return Request.CreateResponse(HttpStatusCode.NotFound, "An entry with the specified key could not be found.");

            // TODO: Should we validate the entry against the specified site and page?

            // Update the properties
            entry.Name = model.Name;
            entry.Email = model.Email;
            entry.Comment = model.Comment;

            // Attempt to add the comment
            UpdateEntryResult result = _feedbackService.UpdateEntry(entry);

            // Return a response matching the result
            switch (result.Status) {

                case AddRatingStatus.Success:
                    return new { key = result.Entry.Key };

                case AddRatingStatus.Cancelled:
                    return Request.CreateResponse(HttpStatusCode.BadRequest, result.Message);

                default:
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, result.Message);

            }

        }

        public object AddComment([FromBody] AddCommentModel model) {

            // Get site site
            if (!_feedbackPluginCollection.TryGetSite(model.SiteKey, out FeedbackSiteSettings site)) {
                return Request.CreateResponse(HttpStatusCode.NotFound, "A site with the specified key could not be found.");
            }
            
            // Get the page
            IPublishedContent page = Umbraco.Content(model.PageKey);
            if (page == null) return Request.CreateResponse(HttpStatusCode.NotFound, "A page with the specified key could not be found.");

            // Get the rating
            if (!site.TryGetRating(model.Rating, out FeedbackRating rating)) {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "A rating with the specified name does not exist.");
            }

            // Attempt to add the comment
            AddCommentResult result = _feedbackService.AddComment(site, page, rating, model.Name, model.Email, model.Comment);

            // Return a response matching the result
            switch (result.Status) {

                case AddCommentStatus.Success:
                    return new { key = result.Entry.Key };

                case AddCommentStatus.Cancelled:
                    return Request.CreateResponse(HttpStatusCode.BadRequest, result.Message);

                default:
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, result.Message);

            }

        }

        #endregion

    }

}