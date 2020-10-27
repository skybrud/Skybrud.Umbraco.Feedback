using Skybrud.Umbraco.Feedback.Models;
using Skybrud.Umbraco.Feedback.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Feedback.Composers {
    
    public class FeedbackComposer : IUserComposer {

        public void Compose(Composition composition) {
            composition.Register<FeedbackDatabaseService>();
            composition.Register<FeedbackService>();
        }

    }

}