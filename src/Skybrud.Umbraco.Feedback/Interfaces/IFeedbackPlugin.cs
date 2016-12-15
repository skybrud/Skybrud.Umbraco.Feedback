using Skybrud.Umbraco.Feedback.Model;
using Skybrud.Umbraco.Feedback.Model.Entries;

namespace Skybrud.Umbraco.Feedback.Interfaces {

    public interface IFeedbackPlugin {

        bool OnEntrySubmitting(FeedbackModule module, FeedbackEntry entry);

        void OnEntrySubmitted(FeedbackModule module, FeedbackEntry entry);

        bool OnRatingSubmitting(FeedbackModule module, FeedbackEntry entry);

        void OnRatingSubmitted(FeedbackModule module, FeedbackEntry entry);

        bool OnEntryUpdating(FeedbackModule module, FeedbackEntry entry);

        void OnEntryUpdated(FeedbackModule module, FeedbackEntry entry);

        bool OnStatusChanging(FeedbackModule module, FeedbackEntry entry, FeedbackStatus newStatus);

        void OnStatusChanged(FeedbackModule module, FeedbackEntry entry, FeedbackStatus oldStatus, FeedbackStatus newStatus);

        bool OnUserAssigning(FeedbackModule module, FeedbackEntry entry, IFeedbackUser newUser);

        void OnUserAssigned(FeedbackModule module, FeedbackEntry entry, IFeedbackUser oldUser, IFeedbackUser newUser);

        void OnEntryResultRender(FeedbackModule module, FeedbackEntryResult result);

        /// <summary>
        /// Gets an array of all available ratings for the site with the specified <code>siteId</code>.
        /// </summary>
        /// <param name="module">A reference to the feedback module.</param>
        /// <param name="siteId">The ID of the site.</param>
        FeedbackRating[] GetRatingsForSite(FeedbackModule module, int siteId);

        /// <summary>
        /// Gets an array of all available statuses for the site with the specified <code>siteId</code>.
        /// </summary>
        /// <param name="module">A reference to the feedback module.</param>
        /// <param name="siteId">The ID of the site.</param>
        FeedbackStatus[] GetStatusesForSite(FeedbackModule module, int siteId);

        IFeedbackUser GetUser(int userId);

        IFeedbackUser[] GetUsers();

    }

}