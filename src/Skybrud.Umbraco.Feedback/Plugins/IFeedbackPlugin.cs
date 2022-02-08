using System;
using System.Collections.Generic;
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
    /// Interface describing a feedback plugin.
    /// </summary>
    public interface IFeedbackPlugin {

        /// <summary>
        /// Method invoked when a new feedback entry is being submitted.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that is being submitted.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        bool OnEntrySubmitting(FeedbackService service, FeedbackEntry entry);

        /// <summary>
        /// Method invoked when a new feedback entry has been submitted.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that was submitted.</param>
        void OnEntrySubmitted(FeedbackService service, FeedbackEntry entry);

        /// <summary>
        /// Method invoked when a new rating for a feedback entry is being submitted.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        bool OnRatingSubmitting(FeedbackService service, FeedbackEntry entry);

        /// <summary>
        /// Method invoked when a new rating for a feedback entry has been submitted.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry.</param>
        void OnRatingSubmitted(FeedbackService service, FeedbackEntry entry);

        /// <summary>
        /// Method invoked when a feedback entry is being updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that is being updated.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        bool OnEntryUpdating(FeedbackService service, FeedbackEntry entry);

        /// <summary>
        /// Method invoked when a feedback entry has been updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that was updated.</param>
        void OnEntryUpdated(FeedbackService service, FeedbackEntry entry);

        /// <summary>
        /// Method invoked when the status of a feedback entry is being updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that is being updated.</param>
        /// <param name="newStatus">The new status of the entry.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        bool OnStatusChanging(FeedbackService service, FeedbackEntry entry, FeedbackStatus newStatus);

        /// <summary>
        /// Method invoked when the status of a feedback entry has been updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry that was updated.</param>
        /// <param name="oldStatus">The status of the entry prior to the update.</param>
        /// <param name="newStatus">The status of the entry after the update.</param>
        void OnStatusChanged(FeedbackService service, FeedbackEntry entry, FeedbackStatus oldStatus, FeedbackStatus newStatus);

        /// <summary>
        /// Method invoked when the assigned user of a feedback entry is changed.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry.</param>
        /// <param name="newUser">A reference to the new user. If the entry is updated to not be assigned to anyone, this value will be <c>null</c>.</param>
        /// <returns><c>true</c> if the feedback plugin handled the entry; otherwise, <c>false</c>.</returns>
        bool OnUserAssigning(FeedbackService service, FeedbackEntry entry, IFeedbackUser newUser);

        /// <summary>
        /// Method invoked when the assigned user of a feedback entry has been updated.
        /// </summary>
        /// <param name="service">A reference to the current feedback service.</param>
        /// <param name="entry">The feedback entry.</param>
        /// <param name="oldUser">The assigned user prior the update.</param>
        /// <param name="newUser">The assigned user after the update.</param>
        void OnUserAssigned(FeedbackService service, FeedbackEntry entry, IFeedbackUser oldUser, IFeedbackUser newUser);

        /// <summary>
        /// Gets the site with the specified <paramref name="key"/>, or <c>null</c> if not found.
        /// </summary>
        /// <param name="key">The key (GUID) of the site.</param>
        /// <param name="site">When this method returns, holds the information about the site if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if a site was found; otherwise, <c>false</c>.</returns>
        bool TryGetSite(Guid key, out FeedbackSiteSettings site);

        /// <summary>
        /// Returns the user with the specified <paramref name="userId"/>, or <c>null</c> if not found.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>An instance of <see cref="IFeedbackUser"/> if successful; otherwise, <c>null</c>.</returns>
        IFeedbackUser GetUser(int userId);

        /// <summary>
        /// Gets the user with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key (GUID) of the user.</param>
        /// <param name="user">When this method returns, holds an instance of <see cref="IFeedbackUser"/> if successful; otherwise <c>false</c>.</param>
        /// <returns><c>true</c> if a user was found; otherwise, <c>false</c>.</returns>
        bool TryGetUser(Guid key, out IFeedbackUser user);

        /// <summary>
        /// Returns an array of all feedback users.
        /// </summary>
        /// <returns>An array of <see cref="IFeedbackUser"/>.</returns>
        IFeedbackUser[] GetUsers();

        /// <summary>
        /// Gets the content app for the specified <paramref name="content"/> item, or <c>null</c> if no feedback
        /// plugins provide a content app for <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The <see cref="IContent"/> to show the content app for.</param>
        /// <param name="userGroups">A list of user groups.</param>
        /// <param name="result">When this method returns, holds the content app if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if a content app was found; otherwise, <c>false</c>.</returns>
        bool TryGetContentApp(IContent content, IEnumerable<IReadOnlyUserGroup> userGroups, out ContentApp result);

    }

}