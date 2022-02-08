namespace Skybrud.Umbraco.Feedback.Services {

    /// <summary>
    /// Enum class indicating the type of entries that should be returned.
    /// </summary>
    public enum FeedbackEntryType {

        /// <summary>
        /// Indicates that all entries should be returned.
        /// </summary>
        All,

        /// <summary>
        /// Indicates that only entries with a rating should be returned.
        /// </summary>
        Rating,
        
        /// <summary>
        /// Indicates that only entries with a comment should be returned.
        /// </summary>
        Comment

    }

}