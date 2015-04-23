using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Skybrud.Umbraco.Feedback.Model;

namespace Skybrud.Umbraco.Feedback.Config {
    
    public class FeedbackConfig {

        #region Properties

        [JsonProperty("email")]
        public FeedbackEmailConfig Email { get; private set; }

        //[JsonProperty("Backend")]
        //public FeedbackBackendConfig Backend { get; set; }

        [JsonProperty("profiles")]
        public Dictionary<int, FeedbackProfile> Profiles { get; private set; }
            
        [JsonProperty("statuses")]
        public FeedbackStatus[] Statuses { get; private set; }

        public static FeedbackConfig Current {
            get {
                FeedbackConfig cfg = HttpContext.Current.Items["SkybrudFeedbackConfig"] as FeedbackConfig;
                if (cfg == null) HttpContext.Current.Items["SkybrudFeedbackConfig"] = cfg = Load();
                return cfg;
            }
        }

        #endregion

        #region Member methods

        public FeedbackProfile GetProfile(int siteId) {
            FeedbackProfile profile;
            if (!Profiles.TryGetValue(siteId, out profile)) {
                Profiles.TryGetValue(0, out profile);
            }
            return profile;
        }

        public bool IsValidStatus(string alias) {
            return Statuses != null && Statuses.Any(x => x.Alias == alias);
        }

        public FeedbackStatus GetStatus(string alias) {
            return Statuses == null ? null : Statuses.FirstOrDefault(x => x.Alias == alias);
        }

        #endregion

        #region Static methods
        
        private static FeedbackConfig Load() {

            string contents = JsonConvert.SerializeObject(new {
                email = new {
                    sendEmail = false,
                    primaryEmail = "@",
                    senderEmail = "noreply@"
                },
                backoffice = new {
                    listLimit = 15
                },
                profiles = new Dictionary<int,object> {
                    {0, new {
                        fields = new {
                            email = "required",
                            name = "optional",
                            comment = "optional"
                        },
                        ratings = new[] {
                            new {
                                name = "Positive",
                                alias = "positive",
                                active = true
                            },
                            new {
                                name = "Negative",
                                alias = "negative",
                                active = true
                            }
                        }
                    }}
                },
                statuses = new[] {
                    new {
                      name = "New",
                      alias = "new",
                      active = true
                    },
                    new {
                      name = "In progress",
                      alias = "inprogress",
                      active = true
                    },
                    new {
                      name = "Closed",
                      alias = "closed",
                      active = true
                    }
                },
            });

            return JsonConvert.DeserializeObject<FeedbackConfig>(contents);

        }

        #endregion

    }

}