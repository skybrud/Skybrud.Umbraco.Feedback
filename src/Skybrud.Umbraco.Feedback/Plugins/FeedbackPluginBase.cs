using System;
using System.Collections.Generic;
using System.Linq;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Skybrud.Umbraco.Feedback.Models.Users;
using Skybrud.Umbraco.Feedback.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.Plugins {

    /// <summary>
    /// Abstract implementation of the <see cref="IFeedbackPlugin"/> interface.
    /// </summary>
    public abstract class FeedbackPluginBase : IFeedbackPlugin {

        /// <summary>
        /// Method invoked when a new feedback entry is being submitted.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that is being submitted.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        public virtual bool OnEntrySubmitting(FeedbackService service, FeedbackEntry entry) {
            return true;
        }

        /// <summary>
        /// Method invoked when a new feedback entry has been submitted.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that was submitted.</param>
        public virtual void OnEntrySubmitted(FeedbackService service, FeedbackEntry entry) { }

        /// <summary>
        /// Method invoked when a new rating for a feedback entry is being submitted.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        public virtual bool OnRatingSubmitting(FeedbackService service, FeedbackEntry entry) {
            return true;
        }

        /// <summary>
        /// Method invoked when a new rating for a feedback entry has been submitted.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry.</param>
        public virtual void OnRatingSubmitted(FeedbackService service, FeedbackEntry entry) { }

        /// <summary>
        /// Method invoked when a feedback entry is being updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that is being updated.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        public virtual bool OnEntryUpdating(FeedbackService service, FeedbackEntry entry) {
            return true;
        }

        /// <summary>
        /// Method invoked when a feedback entry has been updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that was updated.</param>
        public virtual void OnEntryUpdated(FeedbackService service, FeedbackEntry entry) { }

        /// <summary>
        /// Method invoked when the status of a feedback entry is being updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that is being updated.</param>
        /// <param name="newStatus">The new status of the entry.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        public virtual bool OnStatusChanging(FeedbackService service, FeedbackEntry entry, FeedbackStatus newStatus) {
            return true;
        }

        /// <summary>
        /// Method invoked when the status of a feedback entry has been updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that was updated.</param>
        /// <param name="oldStatus">The status of the entry prior to the update.</param>
        /// <param name="newStatus">The status of the entry after the update.</param>
        public virtual void OnStatusChanged(FeedbackService service, FeedbackEntry entry, FeedbackStatus oldStatus, FeedbackStatus newStatus) { }

        /// <summary>
        /// Method invoked when the assigned user of a feedback entry is changed.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry.</param>
        /// <param name="newUser">A reference to the new user. If the entry is updated to not be assigned to anyone, this value will be <c>null</c>.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        public virtual bool OnUserAssigning(FeedbackService service, FeedbackEntry entry, IFeedbackUser newUser) {
            return true;
        }

        /// <summary>
        /// Method invoked when the assigned user of a feedback entry has been updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry.</param>
        /// <param name="oldUser">The assigned user prior the update.</param>
        /// <param name="newUser">The assigned user after the update.</param>
        public virtual void OnUserAssigned(FeedbackService service, FeedbackEntry entry, IFeedbackUser oldUser, IFeedbackUser newUser) { }

        /// <summary>
        /// Gets the site with the specified <paramref name="key"/>, or <c>null</c> if not found.
        /// </summary>
        /// <param name="key">The key (GUID) of the site.</param>
        /// <param name="site">When this method returns, holds the information about the site if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if a site was found; otherwise, <c>false</c>.</returns>
        public virtual bool TryGetSite(Guid key, out FeedbackSiteSettings site) {
            site = null;
            return false;
        }

        /// <summary>
        /// Returns the user with the specified <paramref name="userId"/>, or <c>null</c> if not found.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>An instance of <see cref="IFeedbackUser"/> if successful; otherwise, <c>null</c>.</returns>
        public virtual IFeedbackUser GetUser(int userId) {
            return GetUsers().FirstOrDefault(x => x.Id == userId);
        }

        /// <summary>
        /// Gets the user with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key (GUID) of the user.</param>
        /// <param name="user">When this method returns, holds an instance of <see cref="IFeedbackUser"/> if successful; otherwise <c>false</c>.</param>
        /// <returns><c>true</c> if a user was found; otherwise, <c>false</c>.</returns>
        public virtual bool TryGetUser(Guid key, out IFeedbackUser user) {
            user = GetUsers().FirstOrDefault(x => x.Key == key);
            return user != null;
        }

        /// <summary>
        /// Returns an array of all feedback users.
        /// </summary>
        /// <returns>An array of <see cref="IFeedbackUser"/>.</returns>
        public virtual IFeedbackUser[] GetUsers() {
            return Array.Empty<IFeedbackUser>();
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

        /// <summary>
        /// Returns a content app for the specified <paramref name="site"/>.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>An instance of <see cref="ContentApp"/>.</returns>
        /// <remarks>Override the method and return <c>null</c> for a given site if the content app shouldn't beshown.</remarks>
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

        /// <summary>
        /// Returns a content app for the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="page">The page.</param>
        /// <returns>An instance of <see cref="ContentApp"/>.</returns>
        /// <remarks>Override the method and return <c>null</c> for a given site if the content app shouldn't beshown.</remarks>
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