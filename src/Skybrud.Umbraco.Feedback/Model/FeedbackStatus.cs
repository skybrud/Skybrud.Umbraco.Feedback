using System;
using System.Globalization;
using Newtonsoft.Json;
using Umbraco.Core;

namespace Skybrud.Umbraco.Feedback.Model {

    public class FeedbackStatus {

        [JsonProperty("name")]
        public string Name {
            get {
                if (String.IsNullOrWhiteSpace(Alias)) return "No name";
                string fallback = Alias.ToFirstUpper();
                string translated = ApplicationContext.Current.Services.TextService.Localize("feedback/status" + fallback, CultureInfo.CurrentCulture);
                return String.IsNullOrWhiteSpace(translated) ? fallback : translated;
            }
        }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("active")]
        public bool IsActive { get; set; }

    }

}