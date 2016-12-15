using System.Linq;
using Skybrud.Umbraco.Feedback.Interfaces;
using Skybrud.Umbraco.Feedback.Model.Entries;

namespace Skybrud.Umbraco.Feedback.Model.Plugins {
    
    /// <summary>
    /// Abstract implementation of the <see cref="IFeedbackPlugin"/> interface.
    /// </summary>
    public abstract class FeedbackPluginBase : IFeedbackPlugin {
        
        public virtual bool OnEntrySubmitting(FeedbackModule module, FeedbackEntry entry) {
            return true;
        }

        public virtual void OnEntrySubmitted(FeedbackModule module, FeedbackEntry entry) { }

        public virtual bool OnStatusChanging(FeedbackModule module, FeedbackEntry entry, FeedbackStatus newStatus) {
            return true;
        }

        public virtual bool OnRatingSubmitting(FeedbackModule module, FeedbackEntry entry) {
            return true;
        }

        public virtual void OnRatingSubmitted(FeedbackModule module, FeedbackEntry entry) { }

        public virtual bool OnEntryUpdating(FeedbackModule module, FeedbackEntry entry) {
            return true;
        }

        public virtual void OnEntryUpdated(FeedbackModule module, FeedbackEntry entry) { }

        public virtual void OnStatusChanged(FeedbackModule module, FeedbackEntry entry, FeedbackStatus oldStatus, FeedbackStatus newStatus) { }

        public virtual bool OnUserAssigning(FeedbackModule module, FeedbackEntry entry, IFeedbackUser user) {
            return true;
        }

        public virtual void OnUserAssigned(FeedbackModule module, FeedbackEntry entry, IFeedbackUser oldUser, IFeedbackUser newUser) { }

        public virtual void OnEntryResultRender(FeedbackModule module, FeedbackEntryResult result) { }









        /// <summary>
        /// Gets an array of all available ratings for the site with the specified <code>siteId</code>.
        /// </summary>
        /// <param name="module">A reference to the feedback module.</param>
        /// <param name="siteId">The ID of the site.</param>
        public virtual FeedbackRating[] GetRatingsForSite(FeedbackModule module, int siteId) {
            return new [] {
                FeedbackRating.Positive,
                FeedbackRating.Negative
            };
        }

        /// <summary>
        /// Gets an array of all available statuses for the site with the specified <code>siteId</code>.
        /// </summary>
        /// <param name="module">A reference to the feedback module.</param>
        /// <param name="siteId">The ID of the site.</param>
        public virtual FeedbackStatus[] GetStatusesForSite(FeedbackModule module, int siteId) {
            return new[] {
                FeedbackStatus.New,
                FeedbackStatus.InProgress,
                FeedbackStatus.Closed
            };
        }









        public virtual IFeedbackUser GetUser(int userId) {
            return GetUsers().FirstOrDefault(x => x.Id == userId);
        }

        public virtual IFeedbackUser[] GetUsers() {
            return new IFeedbackUser[0];
        }
    
    }

}