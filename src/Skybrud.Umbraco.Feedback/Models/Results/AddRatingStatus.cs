namespace Skybrud.Umbraco.Feedback.Models.Results {

    /// <summary>
    /// Enum class indicating the status of a <see cref="AddRatingResult"/>.
    /// </summary>
    public enum AddRatingStatus {

        /// <summary>
        /// Indicates that the add operation failed.
        /// </summary>
        Failed,

        /// <summary>
        /// Indicates that the add operation was cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// Indicates that the add operation was successful.
        /// </summary>
        Success

    }

}