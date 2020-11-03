using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Models.Ratings {

    public class FeedbackRating {

        #region Properties

        [JsonProperty("alias")]
        public string Alias { get; }

        [JsonProperty("key")]
        public Guid Key { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("active")]
        public bool IsActive { get; }

        #endregion

        #region Constructors

        public FeedbackRating(Guid key, string alias) {
            Key = key;
            Alias = alias;
            IsActive = true;
        }

        public FeedbackRating(Guid key, string alias, string name) {
            Key = key;
            Alias = alias;
            Name = name;
            IsActive = true;
        }

        #endregion

    }

}