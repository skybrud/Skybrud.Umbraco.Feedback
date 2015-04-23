using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Model {

    public class FeedbackStatus {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("active")]
        public bool IsActive { get; set; }

    }

}