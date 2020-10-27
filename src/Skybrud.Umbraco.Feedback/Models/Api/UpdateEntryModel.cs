using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Models.Api {
        
    public class UpdateEntryModel {

        [JsonProperty("key")]
        public Guid Key { get; set; }

        [JsonProperty("siteKey")]
        public Guid SiteKey { get; set; }

        [JsonProperty("pageKey")]
        public Guid PageKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

    }

}