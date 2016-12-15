using System;
using System.Globalization;
using Newtonsoft.Json;
using Umbraco.Core;

namespace Skybrud.Umbraco.Feedback.Model {

    public class FeedbackRating {

        #region Constants

        public static readonly FeedbackRating Positive = new FeedbackRating("positive");
        
        public static readonly FeedbackRating Negative = new FeedbackRating("negative");

        #endregion

        #region Properties

        [JsonProperty("name")]
        public string Name {
            get {
                if (String.IsNullOrWhiteSpace(Alias)) return "No name";
                string fallback = Alias.ToFirstUpper();
                string translated = ApplicationContext.Current.Services.TextService.Localize("feedback/rating" + fallback, CultureInfo.CurrentCulture);
                return String.IsNullOrWhiteSpace(translated) ? fallback : translated;
            }
        }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("active")]
        public bool IsActive { get; set; }

        #endregion

        #region Constructors

        public FeedbackRating() {
            IsActive = true;
        }

        public FeedbackRating(string alias) : this() {
            Alias = alias;
        }

        #endregion

    }

}