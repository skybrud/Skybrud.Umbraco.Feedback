using Skybrud.Umbraco.Feedback.Components;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Feedback.Composers {
    
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class MigrationComposer : IUserComposer {
        
        public void Compose(Composition composition) {
            composition.Components().Append<MigrationComponent>();
        }

    }

}