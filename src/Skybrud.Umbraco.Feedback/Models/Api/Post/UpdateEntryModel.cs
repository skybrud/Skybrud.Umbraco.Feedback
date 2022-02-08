using Newtonsoft.Json;
using System;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Models.Api.Post {

    public class UpdateEntryModel {

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