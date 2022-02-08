using Skybrud.Umbraco.Feedback.Models.Entries;

namespace Skybrud.Umbraco.Feedback.Models.Results {
    
    /// <summary>
    /// Class representing the result when updating a feedback comment.
    /// </summary>
    public class UpdateEntryResult {

        #region Properties

        /// <summary>
        /// Gets the status of the result.
        /// </summary>
        public AddRatingStatus Status { get; }
        
        /// <summary>
        /// Gets a reference to the entry.
        /// </summary>
        public FeedbackEntry Entry { get; }
        
        /// <summary>
        /// Gets the message of the result - eg. an error message.
        /// </summary>
        public string Message { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="status"/>, <paramref name="entry"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="status">The status of the result.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="message">The message of the result - eg. an error message.</param>
        public UpdateEntryResult(AddRatingStatus status, FeedbackEntry entry, string message) {
            Status = status;
            Entry = entry;
            Message = message;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Initializes a new <see cref="AddRatingStatus.Failed"/> result.
        /// </summary>
        /// <param name="message">An error message about why the comment could not be added.</param>
        /// <returns>An instance of <see cref="UpdateEntryResult"/></returns>
        public static UpdateEntryResult Failed(string message) {
            return new UpdateEntryResult(AddRatingStatus.Failed, null, message);
        }
        
        /// <summary>
        /// Initializes a new <see cref="AddRatingStatus.Cancelled"/> result.
        /// </summary>
        /// <param name="message">A message about why adding the comment was cancelled.</param>
        /// <returns>An instance of <see cref="UpdateEntryResult"/></returns>
        public static UpdateEntryResult Cancelled(string message) {
            return new UpdateEntryResult(AddRatingStatus.Cancelled, null, message);
        }

        /// <summary>
        /// Initializes a new <see cref="AddRatingStatus.Success"/> result.
        /// </summary>
        /// <param name="entry">The entry that was added.</param>
        /// <returns>An instance of <see cref="UpdateEntryResult"/></returns>
        public static UpdateEntryResult Success(FeedbackEntry entry) {
            return new UpdateEntryResult(AddRatingStatus.Success, entry, null);
        }

        #endregion

    }

}