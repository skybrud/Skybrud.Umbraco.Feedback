using System;
using Newtonsoft.Json;

#pragma warning disable 1591

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