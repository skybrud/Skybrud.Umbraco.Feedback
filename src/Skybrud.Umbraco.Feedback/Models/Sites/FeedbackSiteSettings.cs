using System;
using System.Linq;
using Skybrud.Umbraco.Feedback.Constants;
using Skybrud.Umbraco.Feedback.Models.Fields;
using Skybrud.Umbraco.Feedback.Models.Ratings;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.Feedback.Models.Sites {
    
    public class FeedbackSiteSettings {

        #region Properties

        /// <summary>
        /// Gets the numeric ID of the site.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the key (GUID) of the site.
        /// </summary>
        public Guid Key { get; }

        /// <summary>
        /// Gets the name of the site.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the available ratings for this site.
        /// </summary>
        public FeedbackRating[] Ratings { get; }

        /// <summary>
        /// Gets the available statuses for this site.
        /// </summary>
        public FeedbackStatus[] Statuses { get; }

        /// <summary>
        /// Gets the configuration for the fields in the feedback form.
        /// </summary>
        public FeedbackFields Fields { get; }

        #endregion

        #region Constructors

        public FeedbackSiteSettings(IPublishedContent site) {
            
            Id = site.Id;
            Key = site.Key;
            Name = site.Name;

            Ratings = new [] { FeedbackConstants.Ratings.Positive, FeedbackConstants.Ratings.Negative };

            Statuses = new[] { FeedbackConstants.Statuses.New, FeedbackConstants.Statuses.InProgress, FeedbackConstants.Statuses.Closed };

            Fields = new FeedbackFields();

        }

        #endregion

        #region Member methods

        public virtual bool TryGetRating(Guid key, out FeedbackRating rating) {
            rating = Ratings.FirstOrDefault(x => x.Key == key);
            return rating != null;
        }

        public virtual bool TryGetStatus(Guid key, out FeedbackStatus status) {
            status = Statuses.FirstOrDefault(x => x.Key == key);
            return status != null;
        }

        #endregion

    }

}