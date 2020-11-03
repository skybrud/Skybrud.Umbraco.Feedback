using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Feedback.Models.Api.Post {
        
    public class AddCommentModel {

        [JsonProperty("siteKey")]
        public Guid SiteKey { get; set; }

        [JsonProperty("pageKey")]
        public Guid PageKey { get; set; }

        [JsonProperty("rating")]
        public Guid Rating { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

    }

}