using System;
using System.Diagnostics;
using Skybrud.Essentials.Reflection;
using Umbraco.Cms.Core.Semver;

namespace Skybrud.Umbraco.Feedback {

    /// <summary>
    /// Static class with various information and constants about the package.
    /// </summary>
    public static class FeedbackPackage {

        /// <summary>
        /// Gets the alias of the package.
        /// </summary>
        public const string Alias = "Skybrud.Umbraco.Feedback";

        /// <summary>
        /// Gets the friendly name of the package.
        /// </summary>
        public const string Name = "Skybrud Feedback";

        /// <summary>
        /// Gets the version of the package.
        /// </summary>
        public static readonly Version Version = typeof(FeedbackPackage).Assembly.GetName().Version!;

        /// <summary>
        /// Gets the information version of the package.
        /// </summary>
        public static readonly string InformationalVersion = FileVersionInfo
            .GetVersionInfo(typeof(FeedbackPackage).Assembly.Location).ProductVersion!;

        /// <summary>
        /// Gets the semantic version of the package.
        /// </summary>
        public static readonly SemVersion SemVersion = SemVersion.Parse(ReflectionUtils.GetInformationalVersion(typeof(FeedbackPackage)));

        /// <summary>
        /// Gets the URL of the GitHub repository for this package.
        /// </summary>
        public const string GitHubUrl = "https://github.com/skybrud/Skybrud.Umbraco.Feedback";

        /// <summary>
        /// Gets the URL of the issue tracker for this package.
        /// </summary>
        public const string IssuesUrl = "https://github.com/skybrud/Skybrud.Umbraco.Feedback/issues";

        /// <summary>
        /// Gets the URL of the documentation for this package.
        /// </summary>
        public const string DocumentationUrl = "https://github.com/skybrud/Skybrud.Umbraco.Feedback";

    }

}