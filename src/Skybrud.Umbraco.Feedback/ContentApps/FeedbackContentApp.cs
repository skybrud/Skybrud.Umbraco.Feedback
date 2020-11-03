using System.Collections.Generic;
using Skybrud.Umbraco.Feedback.Plugins;
using Umbraco.Core.Models;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.ContentApps {

    public class FeedbackContentApp : IContentAppFactory {

        private readonly FeedbackPluginCollection _pluginCollection;

        public FeedbackContentApp(FeedbackPluginCollection pluginCollection) {
            _pluginCollection = pluginCollection;
        }

        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {
            if (!(source is IContent content)) return null;
            return _pluginCollection.TryGetContentApp(content, userGroups, out ContentApp contentApp) ? contentApp : null;

        }

    }

}