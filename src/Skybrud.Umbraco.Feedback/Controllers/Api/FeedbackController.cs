using Microsoft.AspNetCore.Mvc;
using Skybrud.Umbraco.Feedback.Models.Api.Post;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Skybrud.Umbraco.Feedback.Models.Ratings;
using Skybrud.Umbraco.Feedback.Models.Results;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Plugins;
using Skybrud.Umbraco.Feedback.Services;
using System;
using Skybrud.Umbraco.Feedback.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Controllers.Api {

    public class FeedbackController : UmbracoApiController {

        private readonly FeedbackService _feedbackService;

        private readonly FeedbackPluginCollection _feedbackPluginCollection;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        #region Constructors

        public FeedbackController(FeedbackService feedbackService, FeedbackPluginCollection feedbackPluginCollection, IUmbracoContextAccessor umbracoContextAccessor) {
            _feedbackService = feedbackService;
            _feedbackPluginCollection = feedbackPluginCollection;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        #endregion

        #region Public API methods

        [HttpPost]
        [Route("api/feedback/rating")]
        public object AddRating([FromBody] AddRatingModel model) {

            // Get site site
            if (!_feedbackPluginCollection.TryGetSite(model.SiteKey, out FeedbackSiteSettings site)) {
                return NotFound("A site with the specified key could not be found.");
            }

            // Get the page
            _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext);
            IPublishedContent page = umbracoContext.Content.GetById(model.PageKey);
            if (page == null) {
                return NotFound("A page with the specified key could not be found.");
            }

            // Get the rating
            if (!site.TryGetRating(model.Rating, out FeedbackRating rating)) {
                return BadRequest("A rating with the specified name does not exist.");
            }

            // Attempt to add the rating
            AddRatingResult result = _feedbackService.AddRating(site, page, rating);

            // Return a response matching the result
            switch (result.Status) {

                case AddRatingStatus.Success:
                    return new { key = result.Entry.Key };

                case AddRatingStatus.Cancelled:
                    return BadRequest(result.Message);

                default:
                    throw new Exception(result.Message);

            }

        }

        [HttpPost]
        [Route("api/feedback/comment")]
        public object AddComment([FromBody] AddCommentModel model) {

            // Get site site
            if (!_feedbackPluginCollection.TryGetSite(model.SiteKey, out FeedbackSiteSettings site)) {
                return NotFound("A site with the specified key could not be found.");
            }

            // Get the page
            _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext);
            IPublishedContent page = umbracoContext.Content.GetById(model.PageKey);
            if (page == null) {
                return NotFound("A page with the specified key could not be found.");
            }

            // Get the rating
            if (!site.TryGetRating(model.Rating, out FeedbackRating rating)) {
                return BadRequest("A rating with the specified name does not exist.");
            }

            // Attempt to add the comment
            AddCommentResult result = _feedbackService.AddComment(site, page, rating, model.Name, model.Email, model.Comment);

            // Return a response matching the result
            switch (result.Status) {

                case AddCommentStatus.Success:
                    return new { key = result.Entry.Key };

                case AddCommentStatus.Cancelled:
                    return BadRequest(result.Message);

                default:
                    throw new Exception(result.Message);

            }

        }

        [HttpPost]
        [Route("api/feedback/{key}")]
        public object UpdateEntry(Guid key, [FromBody] UpdateEntryModel model) {

            // Get site site
            if (!_feedbackPluginCollection.TryGetSite(model.SiteKey, out _)) {
                return NotFound("A site with the specified key could not be found.");
            }

            // Get the page
            _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext);
            IPublishedContent page = umbracoContext.Content.GetById(model.PageKey);
            if (page == null) {
                return NotFound("A page with the specified key could not be found.");
            }

            // Get a reference to the entry
            FeedbackEntry entry = _feedbackService.GetEntryByKey(key);
            if (entry == null) {
                return NotFound("An entry with the specified key could not be found.");
            }

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
                    return BadRequest(result.Message);

                default:
                    throw new Exception(result.Message);

            }

        }

        #endregion

    }

}