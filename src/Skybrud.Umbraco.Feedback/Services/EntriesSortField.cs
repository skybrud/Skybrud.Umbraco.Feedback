using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Enums;

namespace Skybrud.Umbraco.Feedback.Services {

    [JsonConverter(typeof(EnumCamelCaseConverter))]
    public enum EntriesSortField {
        CreateDate,
        Rating,
        Status
    }

}