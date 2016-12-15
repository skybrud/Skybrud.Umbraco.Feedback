using System;
using System.Collections.Generic;
using Skybrud.Umbraco.Feedback.Config;
using Skybrud.Umbraco.Feedback.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.Umbraco.Feedback.Model.Entries {

    public class FeedbackEntry {

        #region Private fields

        private FeedbackDatabaseEntry _entry;

        private int _siteId;
        private IPublishedContent _site;

        private int _pageId;
        private IPublishedContent _page;

        private FeedbackRating _rating;
        private FeedbackStatus _status;

        private IFeedbackUser _assignedTo;

        #endregion

        #region Properties

        internal FeedbackDatabaseEntry Row {
            get { return _entry; }
        }

        public int Id {
            get { return _entry.Id; }
            private set { _entry.Id = value; }
        }

        public string UniqueId {
            get { return _entry.UniqueId; }
            internal set { _entry.UniqueId = value; }
        }

        public int SiteId {
            get { return _siteId; }
            set {
                _siteId = value;
                _site = UmbracoContext.Current.ContentCache.GetById(value);
                _entry.SiteId = value;
            }
        }

        public IPublishedContent Site {
            get { return _site; }
            set {
                _site = value;
                _siteId = value == null ? 0 : value.Id;
                _entry.SiteId = _siteId;
            }
        }

        public int PageId {
            get { return _pageId; }
            set {
                _pageId = value;
                _page = UmbracoContext.Current.ContentCache.GetById(value);
                _entry.PageId = value;
            }
        }

        public IPublishedContent Page {
            get { return _page; }
            set {
                _page = value;
                _pageId = value == null ? 0 : value.Id;
                _entry.PageId = _pageId;
            }
        }

        public string Name {
            get { return _entry.Name; }
            set { _entry.Name = value; }
        }

        public string Email {
            get { return _entry.Email; }
            set { _entry.Email = value; }
        }

        public FeedbackRating Rating {
            get { return _rating; }
            set { _rating = value; _entry.Rating = (value == null ? null : _rating.Alias); }
        }

        public FeedbackStatus Status {
            get { return _status; }
            set { _status = value; _entry.Status = (value == null ? null : _status.Alias); }
        }

        public string Comment {
            get { return _entry.Comment; }
            set { _entry.Comment = value; }
        }

        public DateTime Created {
            get { return _entry.Created; }
            set { _entry.Created = value; }
        }

        public DateTime Updated {
            get { return _entry.Updated; }
            set { _entry.Updated = value; }
        }

        public IFeedbackUser AssignedTo {
            get { return _assignedTo; }
            set {
                _assignedTo = value;
                _entry.AssignedTo = (value == null ? -1 : value.Id);
            }
        }

        public bool IsArchived {
            get { return _entry.IsArchived; }
            set { _entry.IsArchived = value; }
        }

        public bool HasComment {
            get { return !String.IsNullOrWhiteSpace(Comment); }
        }

        public bool HasName {
            get { return !String.IsNullOrWhiteSpace(Name); }
        }

        public bool HasEmail {
            get { return !String.IsNullOrWhiteSpace(Email); }
        }

        public bool HasCommentOrNameOrEmail {
            get { return !String.IsNullOrWhiteSpace(Comment + Name + Email); }
        }

        public bool IsRatingOnly {
            get { return !HasCommentOrNameOrEmail; }
        }

        #endregion

        internal FeedbackEntry() {
            _entry = new FeedbackDatabaseEntry();
            _entry.AssignedTo = -1;
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
            Id = Int32.Parse(value + "");
        }

        internal void ChangeStatus(FeedbackStatus status) {
            Status = status;
            _entry.ChangeStatus(status);
        }

        internal void SetAssignedTo(IFeedbackUser user) {
            AssignedTo = user;
            _entry.SetAssignedTo(user);
        }

        internal void Archive() {
            _entry.Archive();
        }
    
    }

}