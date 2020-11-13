using System;

namespace Skybrud.Umbraco.Feedback.Services {

    public class FeedbackGetEntriesOptions {

        public Guid SiteKey { get; set; }

        public int Page { get; set; }

        public int PerPage { get; set; }

        public EntriesSortField SortField { get; set; }

        public EntriesSortOrder SortOrder { get; set; }

        public Guid? Rating { get; set; }

        public Guid? Responsible { get; set; }

        public Guid? Status { get; set; }

        public FeedbackEntryType Type { get; set; }

    }

}