using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.Feedback.Models.Sites;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Models.Api {

    public class SiteApiModel {

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("key")]
        public Guid Key { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("ratings")]
        public RatingApiModel[] Ratings { get; }

        [JsonProperty("statuses")]
        public StatusApiModel[] Statuses { get; }

        public SiteApiModel(FeedbackSiteSettings site, ILocalizedTextService localizedTextService, CultureInfo culture) {
            Id = site.Id;
            Key = site.Key;
            Name = site.Name;
            Ratings = site.Ratings.Select(x => new RatingApiModel(x, localizedTextService, culture)).ToArray();
            Statuses = site.Statuses.Select(x => new StatusApiModel(x, localizedTextService, culture)).ToArray();
        }

    }

}