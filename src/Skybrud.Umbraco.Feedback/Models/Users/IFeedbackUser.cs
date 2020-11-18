using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Models.Users {

    public interface IFeedbackUser {

        [JsonProperty("id")]
        int Id { get; }

        [JsonProperty("key")]
        Guid Key { get; }

        [JsonProperty("name")]
        string Name { get; }

        [JsonProperty("email")]
        string Email { get; }

        [JsonProperty("description")]
        string Description { get; }

        [JsonProperty("avatar")]
        string Avatar { get; }

        [JsonProperty("language")]
        string Language { get; }

    }

}