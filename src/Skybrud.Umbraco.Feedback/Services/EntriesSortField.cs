using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Enums;

namespace Skybrud.Umbraco.Feedback.Services {

    /// <summary>
    /// Enum class indicating the sortable fields of a feedback entry.
    /// </summary>
    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public enum EntriesSortField {

        /// <summary>
        /// Indicates that entries should be sorted by their created date.
        /// </summary>
        CreateDate,

        /// <summary>
        /// Indicates that entries should be sorted by their rating.
        /// </summary>
        Rating,

        /// <summary>
        /// Indicates that entries should be sorted by their status.
        /// </summary>
        Status

    }

}