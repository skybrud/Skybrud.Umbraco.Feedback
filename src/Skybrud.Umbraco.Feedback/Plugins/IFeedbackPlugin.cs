using System;
using Skybrud.Umbraco.Feedback.Model;
using Skybrud.Umbraco.Feedback.Model.Entries;
using Skybrud.Umbraco.Feedback.Models;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Models.Users;
using Skybrud.Umbraco.Feedback.Services;

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

        IFeedbackUser[] GetUsers();

    }

}