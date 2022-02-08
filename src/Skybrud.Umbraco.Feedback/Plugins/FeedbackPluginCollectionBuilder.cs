using Umbraco.Cms.Core.Composing;

namespace Skybrud.Umbraco.Feedback.Plugins {

    public class FeedbackPluginCollectionBuilder : OrderedCollectionBuilderBase<FeedbackPluginCollectionBuilder, FeedbackPluginCollection, IFeedbackPlugin> {

        protected override FeedbackPluginCollectionBuilder This => this;

    }

}