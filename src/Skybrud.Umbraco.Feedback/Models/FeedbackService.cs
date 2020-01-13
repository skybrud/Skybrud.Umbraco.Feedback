using System;
using System.Collections.Generic;
using System.Linq;
using Skybrud.Umbraco.Feedback.Constants;
using Skybrud.Umbraco.Feedback.Exceptions;
using Skybrud.Umbraco.Feedback.Interfaces;
using Skybrud.Umbraco.Feedback.Model;
using Skybrud.Umbraco.Feedback.Model.Entries;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Web;

namespace Skybrud.Umbraco.Feedback.Models {

    /// <summary>
    /// Service class for working with feedback entries.
    /// </summary>
    public class FeedbackService {

        #region Properties

        /// <summary>
        /// Gets a reference to the current database.
        /// </summary>
        protected UmbracoDatabase Database => UmbracoContext.Current.Application.DatabaseContext.Database;
        /// <summary>
        /// Gets a reference to the current <see cref="ISqlSyntaxProvider"/>.
        /// </summary>
        protected ISqlSyntaxProvider SqlSyntax => ApplicationContext.Current.DatabaseContext.SqlSyntax;

        /// <summary>
        /// Gets a reference to the current <see cref="SchemaHelper"/>.
        /// </summary>
        protected DatabaseSchemaHelper SchemaHelper => new DatabaseSchemaHelper(
            ApplicationContext.Current.DatabaseContext.Database,
            ApplicationContext.Current.ProfilingLogger.Logger,
            ApplicationContext.Current.DatabaseContext.SqlSyntax
        );

        #endregion

        #region Public methods

        /// <summary>
        /// Gets an unpaginated array of all feedback entries.
        /// </summary>
        /// <returns>An array of <see cref="FeedbackEntry"/>.</returns>
        public FeedbackEntry[] GetAll() {

            // Return an empty array if the database table doesn't already exist
            if (SchemaHelper.TableExist("SkybrudFeedback") == false) return new FeedbackEntry[0];

            // Call this to make sure the users have been loaded before quering the database (otherwise we might exceptions depending on the database provider)
            Dictionary<int, IFeedbackUser> users = FeedbackModule.Instance.GetUsers();

            //Sql sql = new Sql().Select("*").From("SkybrudFeedback").OrderBy("created DESC");
            Sql sql = new Sql("SELECT * FROM SkybrudFeedback WHERE Archived = 0 ORDER BY created DESC");

            return (
                from entry in Database.Query<FeedbackDatabaseEntry>(sql)
                select new FeedbackEntry(entry, users)
            ).ToArray();

        }

        /// <summary>
        /// Gets an unpaginated array of all feedback entries for the site with the specified <paramref name="siteId"/>.
        /// </summary>
        /// <param name="siteId">The ID of the site.</param>
        /// <returns>An array of <see cref="FeedbackEntry"/>.</returns>
        public FeedbackEntry[] GetAllForSite(int siteId) {

            // Return an empty array if the database table doesn't already exist
            if (SchemaHelper.TableExist("SkybrudFeedback") == false) return new FeedbackEntry[0];

            // Call this to make sure the users have been loaded before quering the database (otherwise we might exceptions depending on the database provider)
            Dictionary<int, IFeedbackUser> users = FeedbackModule.Instance.GetUsers();

            //Sql sql = new Sql().Select("*").From("SkybrudFeedback").Where<FeedbackDatabaseEntry>(x => x.SiteId == siteId && !x.IsArchived).OrderBy("created DESC");
            Sql sql = new Sql("SELECT * FROM SkybrudFeedback WHERE SiteId = @0 AND Archived = 0 ORDER BY created DESC", siteId);

            // Make sure to convert to an array or similar here so the database is queried immidiately (otherwise we might exceptions depending on the database provider)
            FeedbackDatabaseEntry[] entries = Database.Query<FeedbackDatabaseEntry>(sql).ToArray();

            return (
                from entry in entries
                select new FeedbackEntry(entry, users)
            ).ToArray();

        }

        /// <summary>
        /// Gets the entry with the specified <paramref name="entryId"/>.
        /// </summary>
        /// <param name="entryId">The ID of the entry.</param>
        /// <returns>An instance of <see cref="FeedbackEntry"/> or <c>null</c> if not found.</returns>
        public FeedbackEntry GetEntryById(int entryId) {

            // Return NULL if the database table doesn't already exist
            if (SchemaHelper.TableExist("SkybrudFeedback") == false) return null;

            // Call this to make sure the users have been loaded before quering the database (otherwise we might get exceptions depending on the database provider)
            Dictionary<int, IFeedbackUser> users = FeedbackModule.Instance.GetUsers();

            FeedbackDatabaseEntry row = Database.First<FeedbackDatabaseEntry>(
                new Sql()
                    .Select("*")
                    .From("SkybrudFeedback")
                    .Where<FeedbackDatabaseEntry>(x => x.Id == entryId && !x.IsArchived)
            );

            return row == null ? null : new FeedbackEntry(row, users);

        }

        /// <summary>
        /// Archives the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The entry to be archived.</param>
        public void Archive(FeedbackEntry entry) {
            
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            entry._entry.IsArchived = true;

            Save(entry._entry);

        }

        /// <summary>
        /// Updates the status of the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The entry to be updated.</param>
        /// <param name="status">The new status.</param>
        public void SetStatus(FeedbackEntry entry, FeedbackStatus status) {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            if (status == null) throw new ArgumentNullException(nameof(status));
            entry.Status = status;
            Save(entry._entry);
        }

        /// <summary>
        /// Updates the assigned user of the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The entry to be updated.</param>
        /// <param name="user">The user who should be assigned. Use <c>null</c> if the item should be unassigned.</param>
        public void SetAssignedTo(FeedbackEntry entry, IFeedbackUser user) {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            entry.AssignedTo = user;
            Save(entry._entry);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The entry to be delete.</param>
        public void Delete(FeedbackEntry entry) {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            Delete(entry._entry);
        }

        /// <summary>
        /// Deletes all entries before the specified <paramref name="date"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The amount of affected/deleted rows.</returns>
        public int DeleteAll(DateTime date) {
            
            // Delete everything before the start of the day after "date"
            Sql sql = new Sql($"DELETE FROM {FeedbackConstants.TableName} WHERE Created < '{date.Date.AddDays(1):yyyy-MM-dd}';");

            LogHelper.Info<FeedbackService>(sql.SQL);

            return Database.Execute(sql);

        }

        #endregion

        #region Private methods

        private void Save(FeedbackDatabaseEntry entry) {
            Database.Update("SkybrudFeedback", "Id", entry);
        }

        private void Delete(FeedbackDatabaseEntry entry) {
            Database.Delete("SkybrudFeedback", "Id", entry);
        }
        
        #endregion

    }

}