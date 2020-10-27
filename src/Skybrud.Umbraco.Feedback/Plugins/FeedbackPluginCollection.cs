using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.Feedback.Plugins {
    
    public class FeedbackPluginCollection : BuilderCollectionBase<IFeedbackPlugin> {

        /// <summary>
        /// Gets the current plugin collection.
        /// </summary>
        public static FeedbackPluginCollection Current => global::Umbraco.Core.Composing.Current.Factory.GetInstance<FeedbackPluginCollection>();

        /// <summary>
        /// Initializes a new provider collection based on the specified <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to make up the collection.</param>
        public FeedbackPluginCollection(IEnumerable<IFeedbackPlugin> items) : base(items) { }

    }

}