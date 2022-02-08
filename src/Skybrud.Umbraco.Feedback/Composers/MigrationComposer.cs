using Skybrud.Umbraco.Feedback.Components;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Skybrud.Umbraco.Feedback.Composers {

    public class MigrationComposer : IComposer {

        public void Compose(IUmbracoBuilder builder) {
            builder.Components().Append<MigrationComponent>();
        }
    }

}