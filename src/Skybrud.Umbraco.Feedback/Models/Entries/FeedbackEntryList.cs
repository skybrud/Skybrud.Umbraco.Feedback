namespace Skybrud.Umbraco.Feedback.Models.Entries {

    public class FeedbackEntryList {

        public int Page { get; }

        public int PerPage { get; }

        public int Total { get; }

        public FeedbackEntry[] Entries { get; }

        public FeedbackEntryList(int page, int perPage, int total, FeedbackEntry[] entries) {
            Page = page;
            PerPage = perPage;
            Total = total;
            Entries = entries;
        }

    }

}