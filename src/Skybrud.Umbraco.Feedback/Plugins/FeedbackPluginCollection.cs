using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;

namespace Skybrud.Umbraco.Feedback.Plugins {

    public class FeedbackPluginCollection : BuilderCollectionBase<IFeedbackPlugin> {

        /// <summary>
        /// Initializes a new provider collection based on the specified <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to make up the collection.</param>
        public FeedbackPluginCollection(Func<IEnumerable<IFeedbackPlugin>> items) : base(items) { }

    }

}