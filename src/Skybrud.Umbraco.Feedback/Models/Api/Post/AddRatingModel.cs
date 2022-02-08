using Newtonsoft.Json;
using System;

namespace Skybrud.Umbraco.Feedback.Models.Api.Post {

    public class AddRatingModel {

        [JsonProperty("siteKey")]
        public Guid SiteKey { get; set; }

        [JsonProperty("pageKey")]
        public Guid PageKey { get; set; }

        [JsonProperty("rating")]
        public Guid Rating { get; set; }

    }

}