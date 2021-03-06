﻿using System;
using System.Collections.Generic;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;

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

        public static bool TryGetContentApp(this FeedbackPluginCollection collection, IContent content, IEnumerable<IReadOnlyUserGroup> userGroups, out ContentApp result) {

            foreach (IFeedbackPlugin plugin in collection) {
                if (plugin.TryGetContentApp(content, userGroups, out result)) return true;
            }

            result = null;
            return false;

        }

    }

}