using System;
using System.Linq;
using Skybrud.Umbraco.Feedback.Model.Entries;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Models.Users;
using Skybrud.Umbraco.Feedback.Plugins;
using Skybrud.Umbraco.Feedback.Services;

namespace Skybrud.Umbraco.Feedback.Model.Plugins {
    
    /// <summary>
    /// Abstract implementation of the <see cref="IFeedbackPlugin"/> interface.
    /// </summary>
    public abstract class FeedbackPluginBase : IFeedbackPlugin {
        
        public virtual bool OnEntrySubmitting(FeedbackService service, FeedbackEntry entry) {
            return true;
        }

        public virtual void OnEntrySubmitted(FeedbackService service, FeedbackEntry entry) { }

        public virtual bool OnStatusChanging(FeedbackService service, FeedbackEntry entry, FeedbackStatus newStatus) {
            return true;
        }

        public virtual bool OnRatingSubmitting(FeedbackService service, FeedbackEntry entry) {
            return true;
        }

        public virtual void OnRatingSubmitted(FeedbackService service, FeedbackEntry entry) { }

        public virtual bool OnEntryUpdating(FeedbackService service, FeedbackEntry entry) {
            return true;
        }

        public virtual void OnEntryUpdated(FeedbackService service, FeedbackEntry entry) { }

        public virtual void OnStatusChanged(FeedbackService service, FeedbackEntry entry, FeedbackStatus oldStatus, FeedbackStatus newStatus) { }

        public virtual bool OnUserAssigning(FeedbackService service, FeedbackEntry entry, Guid user) {
            return true;
        }

        public virtual void OnUserAssigned(FeedbackService service, FeedbackEntry entry, Guid oldUser, Guid newUser) { }

        //public virtual void OnEntryResultRender(FeedbackService service, FeedbackEntryResult result) { }
        
        public virtual bool TryGetSite(Guid key, out FeedbackSiteSettings site) {
            site = null;
            return false;
        }

        public virtual IFeedbackUser GetUser(int userId) {
            return GetUsers().FirstOrDefault(x => x.Id == userId);
        }

        public virtual IFeedbackUser[] GetUsers() {
            return new IFeedbackUser[0];
        }
    
    }

}