using System;

namespace Skybrud.Umbraco.Feedback.Services {

    /// <summary>
    /// Class with options for getting a list of feedback entries.
    /// </summary>
    public class FeedbackGetEntriesOptions {

        /// <summary>
        /// Gets or sets the key (GUID) of the site.
        /// </summary>
        public Guid SiteKey { get; set; }

        /// <summary>
        /// Gets or sets the key (GUID) of a specific page.
        /// </summary>
        public Guid PageKey { get; set; }

        /// <summary>
        /// Gets or sets the page to be returned.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of items per page.
        /// </summary>
        public int PerPage { get; set; }

        /// <summary>
        /// Gets or sets the field by which the entries should be sorted. Default is <see cref="EntriesSortField.CreateDate"/>.
        /// </summary>
        public EntriesSortField SortField { get; set; }

        /// <summary>
        /// Gets or sets the order by which the entries should be sorted. Default is <see cref="EntriesSortOrder.Asc"/>.
        /// </summary>
        public EntriesSortOrder SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the key (GUID) of rating the returned results should match. Default is <c>null</c>.
        /// </summary>
        public Guid? Rating { get; set; }

        /// <summary>
        /// Gets or sets the key (GUID) of the responsible user the returned results should match. Default is <c>null</c>.
        /// </summary>
        public Guid? Responsible { get; set; }

        /// <summary>
        /// Gets or sets the key (GUID) of the status the returned results should match. Default is <c>null</c>.
        /// </summary>
        public Guid? Status { get; set; }

        /// <summary>
        /// gets or sets the type the returned results should match. Default is <see cref="FeedbackEntryType.All"/>.
        /// </summary>
        public FeedbackEntryType Type { get; set; }

    }

}