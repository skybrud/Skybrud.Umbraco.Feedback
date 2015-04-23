using Skybrud.Umbraco.Feedback.Model;

namespace Skybrud.Umbraco.Feedback.Interfaces {

    public interface IFeedbackPlugin {

        bool OnEntrySubmitting(FeedbackModule module, FeedbackEntry entry);

        void OnEntrySubmitted(FeedbackModule module, FeedbackEntry entry);

        bool OnStatusChanging(FeedbackModule module, FeedbackEntry entry, FeedbackStatus newStatus);

        void OnStatusChanged(FeedbackModule module, FeedbackEntry entry, FeedbackStatus oldStatus, FeedbackStatus newStatus);

        bool OnUserAssigning(FeedbackModule module, FeedbackEntry entry, IFeedbackUser newUser);

        void OnUserAssigned(FeedbackModule module, FeedbackEntry entry, IFeedbackUser oldUser, IFeedbackUser newUser);

        void OnEntryResultRender(FeedbackModule module, FeedbackEntryResult result);

        IFeedbackUser GetUser(int userId);

        IFeedbackUser[] GetUsers();

    }

}