namespace Skybrud.Umbraco.Feedback.Models.Entries {

    /// <summary>
    /// Class representing a paginated list of feedback entries.
    /// </summary>
    public class FeedbackEntryList {

        /// <summary>
        /// Gets the page number of the returned page.
        /// </summary>
        public int Page { get; }
        
        /// <summary>
        /// Gets the maxmimum amount of items that was returned per page.
        /// </summary>
        public int PerPage { get; }

        /// <summary>
        /// Gets the total amount of entries that matched the search.
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// Gets the entries that was returned for the current page.
        /// </summary>
        public FeedbackEntry[] Entries { get; }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="page"/>, <paramref name="perPage"/>, <paramref name="total"/> and <paramref name="entries"/>.
        /// </summary>
        /// <param name="page">The page number of the returned page.</param>
        /// <param name="perPage">The maxmimum amount of items that was returned per page.</param>
        /// <param name="total">The total amount of entries that matched the search.</param>
        /// <param name="entries">The entries that was returned for the current page.</param>
        public FeedbackEntryList(int page, int perPage, int total, FeedbackEntry[] entries) {
            Page = page;
            PerPage = perPage;
            Total = total;
            Entries = entries;
        }

    }

}