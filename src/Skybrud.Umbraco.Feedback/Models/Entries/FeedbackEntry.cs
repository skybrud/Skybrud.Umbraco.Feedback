using System;
using Newtonsoft.Json;
using Skybrud.Umbraco.Feedback.Models.Ratings;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using Skybrud.Umbraco.Feedback.Models.Users;

namespace Skybrud.Umbraco.Feedback.Models.Entries {

    /// <summary>
    /// Class representing a feedback entry.
    /// </summary>
    public class FeedbackEntry {

        #region Private fields

        // ReSharper disable once InconsistentNaming
        internal FeedbackEntryDto _entry;

        private IFeedbackUser _responsible;
        private FeedbackRating _rating;
        private FeedbackStatus _status;

        #endregion

        #region Properties

        public FeedbackEntryDto Dto => _entry;

        /// <summary>
        /// Gets or sets the numeric ID of the entry.
        /// </summary>
        [JsonProperty("id")]
        public int Id => _entry.Id;

        /// <summary>
        /// Gets or sets the key of the entry.
        /// </summary>
        [JsonProperty("key")]
        public Guid Key {
            get => _entry.Key;
            internal set => _entry.Key = value;
        }

        /// <summary>
        /// Gets or sets the key of the site the issue was submitted for.
        /// </summary>
        [JsonProperty("siteKey")]
        public Guid SiteKey {
            get => _entry.SiteKey;
            set => _entry.SiteKey = value;
        }

        /// <summary>
        /// Gets or sets the key of the page the issue was submitted for.
        /// </summary>
        [JsonProperty("pageKey")]
        public Guid PageKey {
            get => _entry.PageKey;
            set => _entry.PageKey = value;
        }

        /// <summary>
        /// Gets or sets the name the user who submitted the entry.
        /// </summary>
        [JsonProperty("name")]
        public string Name {
            get => _entry.Name;
            set => _entry.Name = value;
        }

        /// <summary>
        /// Gets or sets the email address the user who submitted the entry.
        /// </summary>
        [JsonProperty("email")]
        public string Email {
            get => _entry.Email;
            set => _entry.Email = value;
        }

        /// <summary>
        /// Gets or sets the rating of the entry.
        /// </summary>
        [JsonProperty("rating")]
        public FeedbackRating Rating {
            get => _rating;
            set { _rating = value; _entry.Rating = value?.Key ?? Guid.Empty; }
        }

        /// <summary>
        /// Gets or sets the status of the entry.
        /// </summary>
        [JsonProperty("status")]
        public FeedbackStatus Status {
            get => _status;
            set { _status = value; _entry.Status = value?.Key ?? Guid.Empty; }
        }

        /// <summary>
        /// Gets or sets the comment of the entry.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment {
            get => _entry.Comment;
            set => _entry.Comment = value;
        }

        /// <summary>
        /// Gets or sets a timestamp for when the entry was created.
        /// </summary>
        [JsonProperty("createDate")]
        public DateTime CreateDate {
            get => _entry.CreateDate;
            internal set => _entry.CreateDate = value;
        }

        /// <summary>
        /// Gets or sets a timestamp for when the entry was last updated.
        /// </summary>
        [JsonProperty("updateDate")]
        public DateTime UpdateDate {
            get => _entry.UpdateDate;
            set => _entry.UpdateDate = value;
        }

        /// <summary>
        /// Gets or sets the user to which the entry should be assigned.
        /// </summary>
        [JsonProperty("assignedTo")]
        public IFeedbackUser AssignedTo {
            get => _responsible;
            set { _responsible = value; _entry.AssignedTo = value?.Key ?? Guid.Empty; }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("archived")]
        public bool IsArchived {
            get => _entry.IsArchived;
            set => _entry.IsArchived = value;
        }

        /// <summary>
        /// Gets whether the user submitted a comment.
        /// </summary>
        [JsonIgnore]
        public bool HasComment => !string.IsNullOrWhiteSpace(Comment);

        /// <summary>
        /// Gets whether the user submitted their name.
        /// </summary>
        [JsonIgnore]
        public bool HasName => !string.IsNullOrWhiteSpace(Name);

        /// <summary>
        /// Gets whether the user submitted their email address.
        /// </summary>
        [JsonIgnore]
        public bool HasEmail => !string.IsNullOrWhiteSpace(Email);

        /// <summary>
        /// Gets whether the user submitted either a comment, their name or their email address.
        /// </summary>
        [JsonIgnore]
        public bool HasCommentOrNameOrEmail => !string.IsNullOrWhiteSpace(Comment + Name + Email);

        /// <summary>
        /// Gets whether the user only submitted their rating.
        /// </summary>
        [JsonIgnore]
        public bool IsRatingOnly => !HasCommentOrNameOrEmail;

        #endregion

        internal FeedbackEntry() {
            _entry = new FeedbackEntryDto();
        }

        internal FeedbackEntry(FeedbackEntryDto entry) {
            _entry = entry;
        }

        internal FeedbackEntry(FeedbackEntryDto entry, FeedbackRating rating, FeedbackStatus status, IFeedbackUser responsible) {
            _entry = entry;
            _rating = rating;
            _status = status;
            _responsible = responsible;
        }
    
    }

}