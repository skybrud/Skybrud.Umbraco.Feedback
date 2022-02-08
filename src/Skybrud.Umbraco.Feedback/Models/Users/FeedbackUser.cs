using Newtonsoft.Json;
using System;
using System.Globalization;
using Umbraco.Cms.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.Models.Users {
    
    /// <summary>
    /// Class describing a feedback user.
    /// </summary>
    public class FeedbackUser : IFeedbackUser {

        #region Properties
        
        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the key (GUID) of the user.
        /// </summary>
        public Guid Key { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the user.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the avatar of the user.
        /// </summary>
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        
        /// <summary>
        /// Gets or sets the language of the user.
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> of the user.
        /// </summary>
        [JsonIgnore]
        public CultureInfo Culture => CultureInfo.GetCultureInfo(Language.Replace("_", "-"));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new user with default values.
        /// </summary>
        public FeedbackUser() { }

        /// <summary>
        /// Initializes a new instance based on the specified Umbraco <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The Umbraco user the instance should be based on.</param>
        public FeedbackUser(IUser user) {

            Id = user.Id;
            Key = user.Key;
            Name = user.Name;
            Email = user.Email;
            Description = user.Email;
            Language = user.Language;

            if (string.IsNullOrWhiteSpace(user.Avatar)) {
                return;
            }

            Avatar = user.Avatar.StartsWith("UserAvatars/") ? "/media/" + user.Avatar : user.Avatar;

        }

        #endregion

    }

}