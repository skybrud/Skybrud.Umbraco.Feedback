using System.Linq;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Model {
    
    public class FeedbackProfile {

        [JsonProperty("fields")]
        public FeedbackProfileFields Fields { get; set; }

        [JsonProperty("ratings")]
        public FeedbackRating[] Ratings { get; set; }

        #region Member methods

        public bool IsValidRating(string alias) {
            return Ratings != null && Ratings.Any(x => x.Alias == alias);
        }

        public FeedbackRating GetRating(string alias) {
            return Ratings == null ? null : Ratings.FirstOrDefault(x => x.Alias == alias);
        }

        #endregion

    }

    public class FeedbackProfileFields {

        [JsonProperty("email")]
        public FeedbackFieldType Email { get; set; }

        [JsonProperty("name")]
        public FeedbackFieldType Name { get; set; }

        [JsonProperty("comment")]
        public FeedbackFieldType Comment { get; set; }

    }
    
    public enum FeedbackFieldType {
        Required,
        Optional
    }

}