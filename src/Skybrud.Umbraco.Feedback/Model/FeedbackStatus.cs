using System;
using System.Globalization;
using Newtonsoft.Json;
using Umbraco.Core;

namespace Skybrud.Umbraco.Feedback.Model {

    public class FeedbackStatus {

        #region Constants

        public static readonly FeedbackStatus New = new FeedbackStatus { Alias = "new", IsActive = true };

        public static readonly FeedbackStatus InProgress = new FeedbackStatus { Alias = "inprogress", IsActive = true };

        public static readonly FeedbackStatus Closed = new FeedbackStatus { Alias = "closed", IsActive = true };

        #endregion

        #region Properties

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

        #endregion

        #region Constructors

        public FeedbackStatus() {
            IsActive = true;
        }

        public FeedbackStatus(string alias) : this() {
            Alias = alias;
        }

        #endregion

    }

}