using Newtonsoft.Json;
using System;

namespace Skybrud.Umbraco.Feedback.Models.Ratings {
    
    /// <summary>
    /// Class representing a feedback rating.
    /// </summary>
    public class FeedbackRating {

        #region Properties
        
        /// <summary>
        /// Gets the alias of the rating.
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; }
        
        /// <summary>
        /// Gets the key (GUID) of the rating.
        /// </summary>
        [JsonProperty("key")]
        public Guid Key { get; }
        
        /// <summary>
        /// Gets the name of the rating.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }
        
        /// <summary>
        /// Gets whether the rating is active.
        /// </summary>
        [JsonProperty("active")]
        public bool IsActive { get; }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new rating based on the specified <paramref name="key"/> and <paramref name="alias"/>.
        /// </summary>
        /// <param name="key">The key (GUID) of the rating.</param>
        /// <param name="alias">The alias of the rating.</param>
        public FeedbackRating(Guid key, string alias) {
            Key = key;
            Alias = alias;
            IsActive = true;
        }
        
        /// <summary>
        /// Initializes a new rating based on the specified <paramref name="key"/>, <paramref name="alias"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="key">The key (GUID) of the rating.</param>
        /// <param name="alias">The alias of the rating.</param>
        /// <param name="name">The name of the rating.</param>
        public FeedbackRating(Guid key, string alias, string name) {
            Key = key;
            Alias = alias;
            Name = name;
            IsActive = true;
        }

        #endregion

    }

}