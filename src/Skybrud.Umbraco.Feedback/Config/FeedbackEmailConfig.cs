using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Config {

    public class FeedbackEmailConfig {

        [JsonProperty("SendMail")]
        public bool SendEmail { get; private set; }

        [JsonProperty("PrimaryEmail")]
        public string PrimaryEmail { get; private set; }

        [JsonProperty("SenderEmail")]
        public string SenderEmail { get; private set; }

        public FeedbackEmailConfig() {
            SendEmail = false;
            PrimaryEmail = "@";
            SenderEmail = "noreply@";
        }

    }

}