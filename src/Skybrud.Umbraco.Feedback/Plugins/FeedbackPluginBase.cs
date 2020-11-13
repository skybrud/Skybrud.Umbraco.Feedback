using System;
using System.Collections.Generic;
using System.Linq;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Skybrud.Umbraco.Feedback.Models.Users;
using Skybrud.Umbraco.Feedback.Services;
using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.Plugins {
    
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

        public virtual bool OnUserAssigning(FeedbackService service, FeedbackEntry entry, IFeedbackUser user) {
            return true;
        }

        public virtual void OnUserAssigned(FeedbackService service, FeedbackEntry entry, IFeedbackUser oldUser, IFeedbackUser newUser) { }

        //public virtual void OnEntryResultRender(FeedbackService service, FeedbackEntryResult result) { }
        
        public virtual bool TryGetSite(Guid key, out FeedbackSiteSettings site) {
            site = null;
            return false;
        }

        public virtual IFeedbackUser GetUser(int userId) {
            return GetUsers().FirstOrDefault(x => x.Id == userId);
        }

        public virtual bool TryGetUser(Guid key, out IFeedbackUser user) {
            user = GetUsers().FirstOrDefault(x => x.Key == key);
            return user != null;
        }

        public virtual IFeedbackUser[] GetUsers() {
            return new IFeedbackUser[0];
        }

        /// <summary>
        /// Virtual method for getting a feedback content app for the specified <paramref name="content"/>.
        ///
        /// In the default implementation, this method will return <c>false</c> and <paramref name="result"/> will be <c>null</c>.
        /// </summary>
        /// <param name="content">The content item being rendered.</param>
        /// <param name="userGroups">The user groups of the current user.</param>
        /// <param name="result">The content app, or <c>null</c> if a content app shouldn't be sown for <paramref name="content"/>.</param>
        /// <returns><c>true</c> if a content app was configured; otherwise <c>false</c>.</returns>
        public virtual bool TryGetContentApp(IContent content, IEnumerable<IReadOnlyUserGroup> userGroups, out ContentApp result) {
            result = null;
            return false;
        }

        protected virtual ContentApp GetContentAppForSite(IContent site) {

            return new ContentApp {
                Alias = "skybrud-feedback",
                Name = "Feedback",
                Icon = "icon-chat",
                View = "/App_Plugins/Skybrud.Umbraco.Feedback/Views/ContentApp.html",
                ViewModel = new {
                    mode = "site",
                    siteKey = site.Key
                }
            };


        }

        protected virtual ContentApp GetContentAppForPage(IContent site, IContent page) {

            return new ContentApp {
                Alias = "skybrud-feedback",
                Name = "Feedback",
                Icon = "icon-chat",
                View = "/App_Plugins/Skybrud.Umbraco.Feedback/Views/ContentApp.html",
                ViewModel = new {
                    mode = "page",
                    siteKey = site.Key,
                    pageKey = page.Key
                }
            };

        }

    }

}