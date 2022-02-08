using System.Collections.Generic;
using Skybrud.Umbraco.Feedback.Extensions;
using Skybrud.Umbraco.Feedback.Plugins;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.ContentApps {

    /// <summary>
    /// Class representing the Feedback content app.
    /// </summary>
    public class FeedbackContentApp : IContentAppFactory {

        private readonly FeedbackPluginCollection _pluginCollection;

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="pluginCollection"/>.
        /// </summary>
        /// <param name="pluginCollection">A collection of the registered feedback plugins.</param>
        public FeedbackContentApp(FeedbackPluginCollection pluginCollection) {
            _pluginCollection = pluginCollection;
        }

        /// <summary>
        /// Returns the content app, or <c>null</c> if the content app shouldn't be shown in the given context.
        /// </summary>
        /// <param name="source">The source - eg. an instance of <see cref="IContent"/>.</param>
        /// <param name="userGroups">A collection of all user groups.</param>
        /// <returns></returns>
        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {
            if (source is not IContent content) return null;
            return _pluginCollection.TryGetContentApp(content, userGroups, out ContentApp contentApp) ? contentApp : null;

        }

    }

}