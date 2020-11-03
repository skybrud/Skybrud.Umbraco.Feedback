using Skybrud.Umbraco.Feedback.Models.Entries;

namespace Skybrud.Umbraco.Feedback.Models.Results {
    
    public class AddRatingResult {

        public AddRatingStatus Status { get; }

        public FeedbackEntry Entry { get; }

        public string Message { get; }

        public AddRatingResult(AddRatingStatus status, FeedbackEntry entry, string message) {
            Status = status;
            Entry = entry;
            Message = message;
        }

        public static AddRatingResult Failed(string message) {
            return new AddRatingResult(AddRatingStatus.Failed, null, message);
        }

        public static AddRatingResult Cancelled(string message) {
            return new AddRatingResult(AddRatingStatus.Cancelled, null, message);
        }

        public static AddRatingResult Success(FeedbackEntry entry) {
            return new AddRatingResult(AddRatingStatus.Success, entry, null);
        }

    }

}