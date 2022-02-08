using Newtonsoft.Json;
using System;

namespace Skybrud.Umbraco.Feedback.Models.Statuses {

    /// <summary>
    /// Class representing a feedback status.
    /// </summary>
    public class FeedbackStatus {

        #region Properties

        /// <summary>
        /// Gets the alias of the status.
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; }

        /// <summary>
        /// Gets the key (GUID) of the status.
        /// </summary>
        [JsonProperty("key")]
        public Guid Key { get; }

        /// <summary>
        /// Gets the name of the status.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// Gets whether the status is active.
        /// </summary>
        [JsonProperty("active")]
        public bool IsActive { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new status based on the specified <paramref name="key"/> and <paramref name="alias"/>.
        /// </summary>
        /// <param name="key">The key (GUID) of the status.</param>
        /// <param name="alias">The alias of the status.</param>
        public FeedbackStatus(Guid key, string alias) {
            Key = key;
            Alias = alias;
            IsActive = true;
        }

        /// <summary>
        /// Initializes a new status based on the specified <paramref name="key"/>, <paramref name="alias"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="key">The key (GUID) of the status.</param>
        /// <param name="alias">The alias of the status.</param>
        /// <param name="name">The name of the status.</param>
        public FeedbackStatus(Guid key, string alias, string name) {
            Key = key;
            Alias = alias;
            Name = name;
            IsActive = true;
        }

        #endregion

    }

}