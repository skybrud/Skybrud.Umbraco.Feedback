using System;
using System.Collections.Generic;
using Skybrud.Umbraco.Feedback.Config;
using Skybrud.Umbraco.Feedback.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.Umbraco.Feedback.Model.Entries {

    /// <summary>
    /// Class representing a feedback entry.
    /// </summary>
    public class FeedbackEntry {

        #region Private fields

        // ReSharper disable once InconsistentNaming
        internal FeedbackDatabaseEntry _entry;

        private int _siteId;
        private IPublishedContent _site;

        private int _pageId;
        private IPublishedContent _page;

        private FeedbackRating _rating;
        private FeedbackStatus _status;

        private IFeedbackUser _assignedTo;

        #endregion

        #region Properties

        internal FeedbackDatabaseEntry Row => _entry;

        /// <summary>
        /// Gets or sets the numeric ID of the entry.
        /// </summary>
        public int Id {
            get => _entry.Id;
            private set => _entry.Id = value;
        }

        /// <summary>
        /// Gets or sets the unique ID of the entry.
        /// </summary>
        public string UniqueId {
            get => _entry.UniqueId;
            internal set => _entry.UniqueId = value;
        }

        /// <summary>
        /// Gets or sets the ID of the site the issue was submitted for.
        /// </summary>
        public int SiteId {
            get => _siteId;
            set {
                _siteId = value;
                _site = UmbracoContext.Current.ContentCache.GetById(value);
                _entry.SiteId = value;
            }
        }

        /// <summary>
        /// Gets or sets the site the issue was submitted for.
        /// </summary>
        public IPublishedContent Site {
            get => _site;
            set {
                _site = value;
                _siteId = value?.Id ?? 0;
                _entry.SiteId = _siteId;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the page the issue was submitted for.
        /// </summary>
        public int PageId {
            get => _pageId;
            set {
                _pageId = value;
                _page = UmbracoContext.Current.ContentCache.GetById(value);
                _entry.PageId = value;
            }
        }

        /// <summary>
        /// Gets or sets the page the issue was submitted for.
        /// </summary>
        public IPublishedContent Page {
            get => _page;
            set {
                _page = value;
                _pageId = value?.Id ?? 0;
                _entry.PageId = _pageId;
            }
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
            set { _rating = value; _entry.Rating = (value == null ? null : _rating.Alias); }
        }

        /// <summary>
        /// Gets or sets the status of the entry.
        /// </summary>
        public FeedbackStatus Status {
            get => _status;
            set { _status = value; _entry.Status = (value == null ? null : _status.Alias); }
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
            get => _entry.Created;
            internal set => _entry.Created = value;
        }

        /// <summary>
        /// Gets or sets a timestamp for when the entry was last updated.
        /// </summary>
        public DateTime Updated {
            get => _entry.Updated;
            set => _entry.Updated = value;
        }

        /// <summary>
        /// Gets or sets the user to which the entry should be assigned.
        /// </summary>
        public IFeedbackUser AssignedTo {
            get => _assignedTo;
            set {
                _assignedTo = value;
                _entry.AssignedTo = value?.Id ?? -1;
            }
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
            _entry = new FeedbackDatabaseEntry {AssignedTo = -1};
        }

        internal FeedbackEntry(FeedbackDatabaseEntry entry, Dictionary<int, IFeedbackUser> users) {

            _entry = entry;
            
            SiteId = entry.SiteId;
            PageId = entry.PageId;

            var config = FeedbackConfig.Current;
            var profile = FeedbackConfig.Current.GetProfile(entry.SiteId);

            if (profile != null) {
                _rating = profile.GetRating(entry.Rating);
            }

            _status = config.GetStatus(entry.Status);

            _assignedTo = users.ContainsKey(entry.AssignedTo) ? users[entry.AssignedTo] : null;

        }

        internal void Insert() {
            object value = ApplicationContext.Current.DatabaseContext.Database.Insert(_entry);
            Id = int.Parse(value + "");
        }
    
    }

}