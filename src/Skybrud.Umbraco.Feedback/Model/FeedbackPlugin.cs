using System.Linq;
using Skybrud.Umbraco.Feedback.Interfaces;

namespace Skybrud.Umbraco.Feedback.Model {
    
    public abstract class FeedbackPlugin : IFeedbackPlugin {
        
        public virtual bool OnEntrySubmitting(FeedbackModule module, FeedbackEntry entry) {
            return true;
        }

        public virtual void OnEntrySubmitted(FeedbackModule module, FeedbackEntry entry) { }

        public virtual bool OnStatusChanging(FeedbackModule module, FeedbackEntry entry, FeedbackStatus newStatus) {
            return true;
        }

        public virtual void OnStatusChanged(FeedbackModule module, FeedbackEntry entry, FeedbackStatus oldStatus, FeedbackStatus newStatus) { }

        public virtual bool OnUserAssigning(FeedbackModule module, FeedbackEntry entry, IFeedbackUser user) {
            return true;
        }

        public virtual void OnUserAssigned(FeedbackModule module, FeedbackEntry entry, IFeedbackUser oldUser, IFeedbackUser newUser) { }

        public virtual void OnEntryResultRender(FeedbackModule module, FeedbackEntryResult result) { }
        
        public virtual IFeedbackUser GetUser(int userId) {
            return GetUsers().FirstOrDefault(x => x.Id == userId);
        }

        public virtual IFeedbackUser[] GetUsers() {
            return new IFeedbackUser[0];
        }
    
    }

}