using Skybrud.Umbraco.Feedback.ContentApps;
using Skybrud.Umbraco.Feedback.Plugins;
using Skybrud.Umbraco.Feedback.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;

namespace Skybrud.Umbraco.Feedback.Composers {
    
    public class FeedbackComposer : IUserComposer {

        public void Compose(Composition composition) {

            composition.Register<FeedbackDatabaseService>();
            composition.Register<FeedbackService>();

            composition.RegisterUnique<FeedbackPluginCollection>();

            composition.ContentApps().Append<FeedbackContentApp>();

        }

    }

}