using System;
using System.Collections.Generic;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Skybrud.Umbraco.Feedback.Models.Users;
using Skybrud.Umbraco.Feedback.Services;
using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.Plugins {

    public interface IFeedbackPlugin {

        bool OnEntrySubmitting(FeedbackService service, FeedbackEntry entry);

        void OnEntrySubmitted(FeedbackService service, FeedbackEntry entry);

        bool OnRatingSubmitting(FeedbackService service, FeedbackEntry entry);

        void OnRatingSubmitted(FeedbackService service, FeedbackEntry entry);

        bool OnEntryUpdating(FeedbackService service, FeedbackEntry entry);

        void OnEntryUpdated(FeedbackService service, FeedbackEntry entry);

        bool OnStatusChanging(FeedbackService service, FeedbackEntry entry, FeedbackStatus newStatus);

        void OnStatusChanged(FeedbackService service, FeedbackEntry entry, FeedbackStatus oldStatus, FeedbackStatus newStatus);

        bool OnUserAssigning(FeedbackService service, FeedbackEntry entry, Guid newUser);

        void OnUserAssigned(FeedbackService service, FeedbackEntry entry, Guid oldUser, Guid newUser);

        //void OnEntryResultRender(FeedbackService service, FeedbackEntryResult result);

        bool TryGetSite(Guid key, out FeedbackSiteSettings site);

        IFeedbackUser GetUser(int userId);

        bool TryGetUser(Guid key, out IFeedbackUser user);

        IFeedbackUser[] GetUsers();

        bool TryGetContentApp(IContent content, IEnumerable<IReadOnlyUserGroup> userGroups, out ContentApp result);

    }

}