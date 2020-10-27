using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skybrud.Umbraco.Feedback.Model.Entries;

namespace Skybrud.Umbraco.Feedback.Models.Results {
   
    public class UpdateEntryResult {

        public AddRatingStatus Status { get; }

        public FeedbackEntry Entry { get; }

        public string Message { get; }

        public UpdateEntryResult(AddRatingStatus status, FeedbackEntry entry, string message) {
            Status = status;
            Entry = entry;
            Message = message;
        }

        public static UpdateEntryResult Failed(string message) {
            return new UpdateEntryResult(AddRatingStatus.Failed, null, message);
        }

        public static UpdateEntryResult Cancelled(string message) {
            return new UpdateEntryResult(AddRatingStatus.Cancelled, null, message);
        }

        public static UpdateEntryResult Success(FeedbackEntry entry) {
            return new UpdateEntryResult(AddRatingStatus.Success, entry, null);
        }

    }

}