using Skybrud.Umbraco.Feedback.Plugins;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.ContentApps {

    public class FeedbackContentApp : IContentAppFactory {

        private readonly FeedbackPluginCollection _pluginCollection;

        public FeedbackContentApp(FeedbackPluginCollection pluginCollection) {
            _pluginCollection = pluginCollection;
        }

        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {
            if (!(source is IContent content)) {
                return null;
            }

            return _pluginCollection.TryGetContentApp(content, userGroups, out ContentApp contentApp) ? contentApp : null;

        }

    }

}