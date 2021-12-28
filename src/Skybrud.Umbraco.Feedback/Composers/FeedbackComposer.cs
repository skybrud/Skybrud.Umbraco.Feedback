using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.Feedback.ContentApps;
using Skybrud.Umbraco.Feedback.Plugins;
using Skybrud.Umbraco.Feedback.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Feedback.Composers {

    public class FeedbackComposer : IComposer {

        public void Compose(IUmbracoBuilder builder) {
            builder.Services.AddScoped<FeedbackDatabaseService>();
            builder.Services.AddScoped<FeedbackService>();

            builder.FeedbackPlugins();

            builder.ContentApps().Append<FeedbackContentApp>();
        }
    }

}