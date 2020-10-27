using System;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Feedback.Plugins {
    
    /// <summary>
    /// Provides extension methods to the <see cref="Composition"/> class.
    /// </summary>
    public static class FeedbackCompositionExensions {

        /// <summary>
        /// Gets the feedback plugin provider collection builder.
        /// </summary>
        /// <param name="composition">The composition.</param>
        public static FeedbackPluginCollectionBuilder FeedbackPlugins(this Composition composition) {
            return composition.WithCollectionBuilder<FeedbackPluginCollectionBuilder>();
        }

    }

    public static class FeedbackExensions {

        public static bool TryGetSite(this FeedbackPluginCollection collection, Guid key, out FeedbackSiteSettings site) {

            foreach (IFeedbackPlugin plugin in collection) {
                if (plugin.TryGetSite(key, out site)) return true;
            }

            site = null;
            return false;

        }

    }

}