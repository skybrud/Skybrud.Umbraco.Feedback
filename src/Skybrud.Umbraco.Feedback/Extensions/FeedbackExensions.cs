using System;
using System.Collections.Generic;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Skybrud.Umbraco.Feedback.Plugins;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.Extensions {

    /// <summary>
    /// Static class with various Feedback related extension methods.
    /// </summary>
    public static class FeedbackExensions {


        /// <summary>
        /// Gets the site with the specified <paramref name="key"/>, or <c>null</c> if not found.
        ///
        /// The site is found by asking each registered feedback plugin whether they know a site matching
        /// <paramref name="key"/>. The method will return once it's finds the first provider that knows the site.
        /// </summary>
        /// <param name="collection">A collection with the registered feedback plugins.</param>
        /// <param name="key">The key (GUID) of the site.</param>
        /// <param name="site">When this method returns, holds the information about the site if successful; otherwise,
        /// <c>null</c>.</param>
        /// <returns><c>true</c> if a site was found; otherwise, <c>false</c>.</returns>
        public static bool TryGetSite(this FeedbackPluginCollection collection, Guid key, out FeedbackSiteSettings site) {

            foreach (IFeedbackPlugin plugin in collection) {
                if (plugin.TryGetSite(key, out site)) {
                    return true;
                }
            }

            site = null;
            return false;

        }

        /// <summary>
        /// Attempts to get the parent site of the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="collection">A collection with the registered feedback plugins.</param>
        /// <param name="content">The content representing a page under the site.</param>
        /// <param name="site">When this method returns, holds an instance of <see cref="FeedbackSiteSettings"/> representing the parent site if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetSite(this FeedbackPluginCollection collection, IContent content, out FeedbackSiteSettings site) {

            foreach (IFeedbackPlugin plugin in collection) {
                if (plugin.TryGetSite(content, out site)) {
                    return true;
                }
            }

            site = null;
            return false;

        }

        /// <summary>
        /// Gets the content app for the specified <paramref name="content"/> item, or <c>null</c> if no feedback
        /// plugins provide a content app for <paramref name="content"/>.
        ///
        /// The content app is found by asking each registered feedback plugin whether they provide a content app for
        /// <paramref name="content"/>. The method will return once it's finds the first provider that returns a
        /// content app.
        /// </summary>
        /// <param name="collection">A collection with the registered feedback plugins.</param>
        /// <param name="content">The <see cref="IContent"/> to show the content app for.</param>
        /// <param name="userGroups">A list of user groups.</param>
        /// <param name="result">When this method returns, holds the content app if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if a content app was found; otherwise, <c>false</c>.</returns>
        public static bool TryGetContentApp(this FeedbackPluginCollection collection, IContent content, IEnumerable<IReadOnlyUserGroup> userGroups, out ContentApp result) {

            foreach (IFeedbackPlugin plugin in collection) {
                if (plugin.TryGetContentApp(content, userGroups, out result)) {
                    return true;
                }
            }

            result = null;
            return false;

        }

    }

}