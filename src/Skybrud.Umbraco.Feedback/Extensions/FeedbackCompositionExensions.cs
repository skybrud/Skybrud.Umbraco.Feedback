using Umbraco.Cms.Core.DependencyInjection;

namespace Skybrud.Umbraco.Feedback.Plugins {

    /// <summary>
    /// Provides extension methods to the <see cref="IUmbracoBuilder"/> class.
    /// </summary>
    public static class FeedbackCompositionExensions {

        /// <summary>
        /// Gets the feedback plugin provider collection builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static FeedbackPluginCollectionBuilder FeedbackPlugins(this IUmbracoBuilder builder) {
            return builder.WithCollectionBuilder<FeedbackPluginCollectionBuilder>();
        }

    }

}