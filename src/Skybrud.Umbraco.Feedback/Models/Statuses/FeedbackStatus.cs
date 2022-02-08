using Newtonsoft.Json;
using System;

namespace Skybrud.Umbraco.Feedback.Models.Statuses {

    public class FeedbackStatus {

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

        public FeedbackStatus(Guid key, string alias) {
            Key = key;
            Alias = alias;
            IsActive = true;
        }

        public FeedbackStatus(Guid key, string alias, string name) {
            Key = key;
            Alias = alias;
            Name = name;
            IsActive = true;
        }

        #endregion

    }

}