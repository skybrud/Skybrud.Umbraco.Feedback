using System;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Enums;

namespace Skybrud.Umbraco.Feedback.Services {


    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public enum EntriesSortField {
        CreateDate,
        Rating,
        Status
    }


    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public enum EntriesSortOrder {
        Asc,
        Desc
    }

    public class FeedbackGetEntriesOptions {

        public Guid SiteKey { get; set; }

        public int Page { get; set; }

        public int PerPage { get; set; }

        public EntriesSortField SortField { get; set; }

        public EntriesSortOrder SortOrder { get; set; }

    }

}