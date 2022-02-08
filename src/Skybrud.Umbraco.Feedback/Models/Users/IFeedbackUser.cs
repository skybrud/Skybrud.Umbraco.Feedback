using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Models.Users {

    /// <summary>
    /// Interface describing a feedback user.
    /// </summary>
    public interface IFeedbackUser {

        /// <summary>
        /// Gets the ID of the user.
        /// </summary>
        [JsonProperty("id")]
        int Id { get; }

        /// <summary>
        /// Gets the key (GUID) of the user.
        /// </summary>
        [JsonProperty("key")]
        Guid Key { get; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        [JsonProperty("name")]
        string Name { get; }

        /// <summary>
        /// Gets the email address of the user.
        /// </summary>
        [JsonProperty("email")]
        string Email { get; }

        /// <summary>
        /// Gets the description of the user.
        /// </summary>
        [JsonProperty("description")]
        string Description { get; }

        /// <summary>
        /// Gets the avatar of the user.
        /// </summary>
        [JsonProperty("avatar")]
        string Avatar { get; }

        /// <summary>
        /// Gets the language of the user.
        /// </summary>
        [JsonProperty("language")]
        string Language { get; }

    }

}