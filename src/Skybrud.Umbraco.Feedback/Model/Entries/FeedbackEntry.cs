using System;

namespace Skybrud.Umbraco.Feedback.Model.Entries {

    /// <summary>
    /// Class representing a feedback entry.
    /// </summary>
    public class FeedbackEntry {

        #region Private fields

        // ReSharper disable once InconsistentNaming
        internal FeedbackEntryDto _entry;

        private FeedbackRating _rating;
        private FeedbackStatus _status;

        #endregion

        #region Properties

        internal FeedbackEntryDto Dto => _entry;

        /// <summary>
        /// Gets or sets the numeric ID of the entry.
        /// </summary>
        public int Id {
            get => _entry.Id;
            private set => _entry.Id = value;
        }

        /// <summary>
        /// Gets or sets the key of the entry.
        /// </summary>
        public Guid Key {
            get => _entry.Key;
            internal set => _entry.Key = value;
        }

        /// <summary>
        /// Gets or sets the key of the site the issue was submitted for.
        /// </summary>
        public Guid SiteKey {
            get => _entry.SiteKey;
            set => _entry.SiteKey = value;
        }

        /// <summary>
        /// Gets or sets the key of the page the issue was submitted for.
        /// </summary>
        public Guid PageKey {
            get => _entry.PageKey;
            set => _entry.PageKey = value;
        }

        /// <summary>
        /// Gets or sets the name the user who submitted the entry.
        /// </summary>
        public string Name {
            get => _entry.Name;
            set => _entry.Name = value;
        }

        /// <summary>
        /// Gets or sets the email address the user who submitted the entry.
        /// </summary>
        public string Email {
            get => _entry.Email;
            set => _entry.Email = value;
        }

        /// <summary>
        /// Gets or sets the rating of the entry.
        /// </summary>
        public FeedbackRating Rating {
            get => _rating;
            set { _rating = value; _entry.Rating = (value == null ? Guid.Empty : _rating.Key); }
        }

        /// <summary>
        /// Gets or sets the status of the entry.
        /// </summary>
        public FeedbackStatus Status {
            get => _status;
            set { _status = value; _entry.Status = (value == null ? Guid.Empty : _status.Key); }
        }

        /// <summary>
        /// Gets or sets the comment of the entry.
        /// </summary>
        public string Comment {
            get => _entry.Comment;
            set => _entry.Comment = value;
        }

        /// <summary>
        /// Gets or sets a timestamp for when the entry was created.
        /// </summary>
        public DateTime Created {
            get => _entry.CreateDate;
            internal set => _entry.CreateDate = value;
        }

        /// <summary>
        /// Gets or sets a timestamp for when the entry was last updated.
        /// </summary>
        public DateTime UpdateDate {
            get => _entry.UpdateDate;
            set => _entry.UpdateDate = value;
        }

        /// <summary>
        /// Gets or sets the user to which the entry should be assigned.
        /// </summary>
        public Guid AssignedTo {
            get => _entry.AssignedTo;
            set => _entry.AssignedTo = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsArchived {
            get => _entry.IsArchived;
            set => _entry.IsArchived = value;
        }

        /// <summary>
        /// Gets whether the user submitted a comment.
        /// </summary>
        public bool HasComment => !string.IsNullOrWhiteSpace(Comment);

        /// <summary>
        /// Gets whether the user submitted their name.
        /// </summary>
        public bool HasName => !string.IsNullOrWhiteSpace(Name);

        /// <summary>
        /// Gets whether the user submitted their email address.
        /// </summary>
        public bool HasEmail => !string.IsNullOrWhiteSpace(Email);

        /// <summary>
        /// Gets whether the user submitted either a comment, their name or their email address.
        /// </summary>
        public bool HasCommentOrNameOrEmail => !string.IsNullOrWhiteSpace(Comment + Name + Email);

        /// <summary>
        /// Gets whether the user only submitted their rating.
        /// </summary>
        public bool IsRatingOnly => !HasCommentOrNameOrEmail;

        #endregion

        internal FeedbackEntry() {
            _entry = new FeedbackEntryDto();
        }

        internal FeedbackEntry(FeedbackEntryDto entry) {
            _entry = entry;
        }
    
    }

}