using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Interfaces {

    public interface IFeedbackUser {

        [JsonProperty("id")]
        int Id { get; }

        [JsonProperty("name")]
        string Name { get; }

        [JsonProperty("email")]
        string Email { get; }

        [JsonProperty("language")]
        string Language { get; }

    }

}