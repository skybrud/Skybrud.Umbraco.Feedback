using Newtonsoft.Json;
using System;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Models.Api {

    public class PageApiModel {

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("key")]
        public Guid Key { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("published")]
        public bool IsPublished { get; }

        [JsonProperty("url")]
        public string Url { get; }

        public PageApiModel(IPublishedContent content) {
            Id = content.Id;
            Key = content.Key;
            Name = content.Name;
            IsPublished = true;
            Url = content.Url();
        }

        public PageApiModel(IContent content) {
            Id = content.Id;
            Key = content.Key;
            Name = content.Name;
            IsPublished = content.Published;
        }

    }

}