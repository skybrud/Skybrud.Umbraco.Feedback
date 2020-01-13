using System;
using System.Globalization;
using Newtonsoft.Json;
using Skybrud.Umbraco.Feedback.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Web;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Model.Entries {

    public class FeedbackEntryResult {

        public class PageInfo {

            [JsonIgnore]
            public IPublishedContent Content { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            public static PageInfo GetFromContent(IPublishedContent content) {
                if (content == null) return null;
                return new PageInfo {
                    Content = content,
                    Id = content.Id,
                    Name = content.Name,
                    Url = content.UrlWithDomain()
                };
            }

        }

        public class B {

            [JsonProperty("name")]
            public string Name { get; set; }
            
            [JsonProperty("alias")]
            public string Alias { get; set; }

            public static B GetFromStatus(FeedbackStatus status) {
                if (status == null) return null;
                return new B {
                    Name = status.Name,
                    Alias = status.Alias
                };
            }

            public static B GetFromRating(FeedbackRating rating) {
                if (rating == null) return null;
                return new B {
                    Name = rating.Name,
                    Alias = rating.Alias
                };
            }

        }

        private FeedbackEntryResult(FeedbackEntry entry, UmbracoHelper helper, CultureInfo culture) {

            IPublishedContent content = helper.TypedContent(entry.PageId);
            
            Entry = entry;
            Id = entry.Id;
            SiteId = entry.SiteId;
            PageId = entry.PageId;
            Page = PageInfo.GetFromContent(content);
            Name = entry.Name;
            Email = entry.Email;
            Rating = B.GetFromRating(entry.Rating);
            Comment = String.IsNullOrWhiteSpace(entry.Comment) ? new string[0] : entry.Comment.Trim().Split('\n');
            Status = B.GetFromStatus(entry.Status);
            CreatedDateTime = entry.Created;
            Created = entry.Created.ToLocalTime().ToString("dd-MM-yyyy HH:mm", culture);
            AssignedTo = entry.AssignedTo;

        }

        public static FeedbackEntryResult GetFromEntry(FeedbackEntry entry, UmbracoHelper helper, CultureInfo culture) {
            return new FeedbackEntryResult(entry, helper, culture);
        }

        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("siteId")]
        public int SiteId { get; private set; }

        [JsonProperty("pageId")]
        public int PageId { get; private set; }

        [JsonProperty("page")]
        public PageInfo Page { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("email")]
        public string Email { get; private set; }

        [JsonProperty("rating")]
        public B Rating { get; private set; }

        [JsonProperty("comment")]
        public string[] Comment { get; private set; }

        [JsonProperty("created")]
        public string Created { get; private set; }

        [JsonProperty("status")]
        public B Status { get; private set; }

        [JsonIgnore]
        public FeedbackEntry Entry { get; private set; }

        [JsonIgnore]
        public DateTime CreatedDateTime { get; private set; }

        [JsonProperty("assignedTo")]
        public IFeedbackUser AssignedTo { get; private set; }

    }

}