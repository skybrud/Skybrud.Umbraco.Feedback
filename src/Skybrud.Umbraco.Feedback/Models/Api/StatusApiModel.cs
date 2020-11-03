using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Umbraco.Core.Services;

namespace Skybrud.Umbraco.Feedback.Models.Api {

    public class StatusApiModel {

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

        public StatusApiModel(FeedbackStatus status, HttpRequestBase request, ILocalizedTextService localizedTextService, CultureInfo culture) {

            Alias = status.Alias;
            Key = status.Key;
            Name = status.Name;
            IsActive = status.IsActive;

            if (string.IsNullOrWhiteSpace(status.Name)) {
                Name += localizedTextService.Localize($"feedback/status{Alias.ToPascalCase()}", culture);
            }

        }

        #endregion

    }
}
