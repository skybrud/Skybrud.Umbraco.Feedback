using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Enums;

namespace Skybrud.Umbraco.Feedback.Services {

    /// <summary>
    /// Enum class indicating the sort order of a list of entries.
    /// </summary>
    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public enum EntriesSortOrder {

        /// <summary>
        /// Indicates that entries should be sorted in ascending order.
        /// </summary>
        Asc,

        /// <summary>
        /// Indicates that entries should be sorted in descending order.
        /// </summary>
        Desc

    }

}