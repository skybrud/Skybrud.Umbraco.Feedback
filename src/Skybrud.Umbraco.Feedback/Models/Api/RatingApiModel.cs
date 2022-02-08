using Newtonsoft.Json;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Feedback.Models.Ratings;
using System;
using System.Globalization;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Models.Api {

    public class RatingApiModel {

        #region Properties

        [JsonProperty("alias")]
        public string Alias { get; }

        [JsonProperty("key")]
        public Guid Key { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("active")]
        public bool IsActive { get; }

        #endregion

        #region Constructors

        public RatingApiModel(FeedbackRating rating, ILocalizedTextService localizedTextService, CultureInfo culture) {

            Alias = rating.Alias;
            Key = rating.Key;
            Name = rating.Name;
            IsActive = rating.IsActive;

            if (string.IsNullOrWhiteSpace(rating.Name)) {
                Name += localizedTextService.Localize("feedback", $"rating{Alias.ToPascalCase()}", culture);
            }

        }

        #endregion

    }

}