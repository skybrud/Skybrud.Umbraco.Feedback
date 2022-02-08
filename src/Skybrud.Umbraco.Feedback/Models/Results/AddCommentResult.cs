using Skybrud.Umbraco.Feedback.Models.Entries;

namespace Skybrud.Umbraco.Feedback.Models.Results {

    /// <summary>
    /// Class representing the result when adding a new feedback comment.
    /// </summary>
    public class AddCommentResult {

        #region Properties

        /// <summary>
        /// Gets the status of the result.
        /// </summary>
        public AddCommentStatus Status { get; }

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
        public AddCommentResult(AddCommentStatus status, FeedbackEntry entry, string message) {
            Status = status;
            Entry = entry;
            Message = message;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Initializes a new <see cref="AddCommentStatus.Failed"/> result.
        /// </summary>
        /// <param name="message">An error message about why the comment could not be added.</param>
        /// <returns>An instance of <see cref="AddCommentResult"/></returns>
        public static AddCommentResult Failed(string message) {
            return new AddCommentResult(AddCommentStatus.Failed, null, message);
        }
        
        /// <summary>
        /// Initializes a new <see cref="AddCommentStatus.Cancelled"/> result.
        /// </summary>
        /// <param name="message">A message about why adding the comment was cancelled.</param>
        /// <returns>An instance of <see cref="AddCommentResult"/></returns>
        public static AddCommentResult Cancelled(string message) {
            return new AddCommentResult(AddCommentStatus.Cancelled, null, message);
        }

        /// <summary>
        /// Initializes a new <see cref="AddCommentStatus.Success"/> result.
        /// </summary>
        /// <param name="entry">The entry that was added.</param>
        /// <returns>An instance of <see cref="AddCommentResult"/></returns>
        public static AddCommentResult Success(FeedbackEntry entry) {
            return new AddCommentResult(AddCommentStatus.Success, entry, null);
        }

        #endregion

    }

}