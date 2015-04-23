using System.Globalization;
using Newtonsoft.Json;
using Skybrud.Umbraco.Feedback.Interfaces;

namespace Skybrud.Umbraco.Feedback.Model {
    
    public class FeedbackUser : IFeedbackUser {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Language { get; set; }

        [JsonIgnore]
        public CultureInfo Culture {
            get { return CultureInfo.GetCultureInfo(Language.Replace("_", "-")); }
        }
    
    }

}