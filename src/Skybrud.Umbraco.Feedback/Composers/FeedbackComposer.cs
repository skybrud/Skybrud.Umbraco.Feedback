using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.Feedback.ContentApps;
using Skybrud.Umbraco.Feedback.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Skybrud.Umbraco.Feedback.Composers {

#pragma warning disable 1591

    public class FeedbackComposer : IComposer {

        public void Compose(IUmbracoBuilder builder) {

            builder.Services.AddScoped<FeedbackDatabaseService>();
            builder.Services.AddScoped<FeedbackService>();

            builder.ContentApps().Append<FeedbackContentApp>();

        }

    }

}