using System;
using System.Globalization;
using Newtonsoft.Json;
using Umbraco.Core.Models.Membership;

namespace Skybrud.Umbraco.Feedback.Models.Users {
    
    public class FeedbackUser : IFeedbackUser {

        public int Id { get; set; }
        
        public Guid Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Avatar { get; set; }


        public string Language { get; set; }

        [JsonIgnore]
        public CultureInfo Culture => CultureInfo.GetCultureInfo(Language.Replace("_", "-"));

        public FeedbackUser() { }

        public FeedbackUser(IUser user) {
            Id = user.Id;
            Key = user.Key;
            Name = user.Name;
            Description = user.Email;
            Language = user.Language;

            if (string.IsNullOrWhiteSpace(user.Avatar)) return;
            Avatar = user.Avatar.StartsWith("UserAvatars/") ? "/media/" + user.Avatar : user.Avatar;

        }

    }

}