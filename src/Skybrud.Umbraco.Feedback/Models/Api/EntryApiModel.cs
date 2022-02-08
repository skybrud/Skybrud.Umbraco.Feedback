using System;
using Newtonsoft.Json;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Skybrud.Umbraco.Feedback.Models.Users;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Models.Api {

    public class EntryApiModel {

        protected FeedbackEntry Entry { get; }

        [JsonProperty("id")]
        public int Id => Entry.Id;

        [JsonProperty("key")]
        public Guid Key => Entry.Key;

        [JsonProperty("site")]
        public SiteApiModel Site { get; }

        [JsonProperty("page")]
        public PageApiModel Page { get; }

        [JsonProperty("name")]
        public string Name => Entry.Name;

        [JsonProperty("email")]
        public string Email => Entry.Email;

        [JsonProperty("comment")]
        public string Comment => Entry.Comment;

        [JsonProperty("status")]
        public StatusApiModel Status { get; }

        [JsonProperty("rating")]
        public RatingApiModel Rating { get; }

        [JsonProperty("assignedTo")]
        public IFeedbackUser AssignedTo { get; }

        [JsonProperty("createDate")]
        public DateTime CreateDate => Entry.CreateDate;

        [JsonProperty("updateDate")]
        public DateTime UpdateDate => Entry.UpdateDate;

        [JsonProperty("archived")]
        public bool IsArchived => Entry.IsArchived;

        public EntryApiModel(FeedbackEntry entry, SiteApiModel site, PageApiModel page, StatusApiModel status, RatingApiModel rating, IFeedbackUser assignedTo) {
            Entry = entry;
            Site = site;
            Page = page;
            Status = status;
            Rating = rating;
            AssignedTo = assignedTo;
        }

    }

}