using Skybrud.Umbraco.Feedback.Models.Entries;

namespace Skybrud.Umbraco.Feedback.Models.Results {

    public class AddCommentResult {

        public AddCommentStatus Status { get; }

        public FeedbackEntry Entry { get; }

        public string Message { get; }

        public AddCommentResult(AddCommentStatus status, FeedbackEntry entry, string message) {
            Status = status;
            Entry = entry;
            Message = message;
        }

        public static AddCommentResult Failed(string message) {
            return new AddCommentResult(AddCommentStatus.Failed, null, message);
        }

        public static AddCommentResult Cancelled(string message) {
            return new AddCommentResult(AddCommentStatus.Cancelled, null, message);
        }

        public static AddCommentResult Success(FeedbackEntry entry) {
            return new AddCommentResult(AddCommentStatus.Success, entry, null);
        }

    }

}