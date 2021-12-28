using Microsoft.Extensions.Logging;
using NPoco;
using Skybrud.Umbraco.Feedback.Constants;
using Skybrud.Umbraco.Feedback.Extensions;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Skybrud.Umbraco.Feedback.Plugins;
using System;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Feedback.Services {

    public class FeedbackDatabaseService {

        private readonly IScopeProvider _scopeProvider;

        private readonly ILogger _logger;

        #region Properties

        protected FeedbackPluginCollection Plugins { get; }

        #endregion

        #region Constructors

        public FeedbackDatabaseService(IScopeProvider scopeProvider, ILogger logger, FeedbackPluginCollection feedbackPlugins) {
            _scopeProvider = scopeProvider;
            _logger = logger;
            Plugins = feedbackPlugins;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets an unpaginated array of all feedback entries.
        /// </summary>
        /// <returns>An array of <see cref="FeedbackEntryDto"/>.</returns>
        public FeedbackEntryDto[] GetAllEntries() {

            using (var scope = _scopeProvider.CreateScope()) {

                // Declare the SQL for the query
                var sql = new Sql($"SELECT * FROM {FeedbackConstants.TableName} WHERE Archived = 0 ORDER BY created DESC");

                // Make the call to the database
                return scope.Database
                    .Fetch<FeedbackEntryDto>(sql)
                    .ToArray();

            }

        }

        /// <summary>
        /// Gets an unpaginated array of all feedback entries for the site with the specified <paramref name="siteId"/>.
        /// </summary>
        /// <param name="siteId">The ID of the site.</param>
        /// <returns>An array of <see cref="FeedbackEntry"/>.</returns>
        public FeedbackEntryDto[] GetAllEntriesForSite(int siteId) {

            using (var scope = _scopeProvider.CreateScope()) {

                // Declare the SQL for the query
                var sql = new Sql($"SELECT * FROM {FeedbackConstants.TableName} WHERE SiteId = @0 AND Archived = 0 ORDER BY created DESC", siteId);

                // Make the call to the database
                return scope.Database
                    .Fetch<FeedbackEntryDto>(sql)
                    .ToArray();

            }

        }

        public FeedbackEntryDto[] GetEntries(FeedbackGetEntriesOptions options, out int total) {

            using (var scope = _scopeProvider.CreateScope()) {

                // Declare the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<FeedbackEntryDto>()
                    .From<FeedbackEntryDto>();

                sql.Where<FeedbackEntryDto>(x => !x.IsArchived);

                if (options.Rating != null) sql.Where<FeedbackEntryDto>(x => x.Rating == options.Rating);
                if (options.Responsible != null) sql.Where<FeedbackEntryDto>(x => x.AssignedTo == options.Responsible);
                if (options.Status != null) sql.Where<FeedbackEntryDto>(x => x.Status == options.Status);

                if (options.Type == FeedbackEntryType.Comment) sql.Where<FeedbackEntryDto>(x => x.Comment != null);
                if (options.Type == FeedbackEntryType.Rating) sql.Where<FeedbackEntryDto>(x => x.Comment == null);

                if (options.SiteKey != Guid.Empty) sql = sql.Where<FeedbackEntryDto>(x => x.SiteKey == options.SiteKey);

                switch (options.SortField) {

                    case EntriesSortField.Rating:
                        sql = sql.OrderBy<FeedbackEntryDto>(x => x.Rating, options.SortOrder);
                        break;

                    case EntriesSortField.Status:
                        sql = sql.OrderBy<FeedbackEntryDto>(x => x.Status, options.SortOrder);
                        break;

                    default:
                        sql = sql.OrderBy<FeedbackEntryDto>(x => x.CreateDate, options.SortOrder);
                        break;

                }

                // Make the call to the database
                return scope.Database.Page<FeedbackEntryDto>(options.Page, options.PerPage, sql, out total);

            }


        }




        public FeedbackEntryDto[] GetEntriesForSite(Guid siteKey, int limit, int page, out int total) {

            using (var scope = _scopeProvider.CreateScope()) {

                // Declare the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<FeedbackEntryDto>()
                    .From<FeedbackEntryDto>()
                    .Where<FeedbackEntryDto>(x => x.SiteKey == siteKey && x.IsArchived == false)
                    .OrderByDescending<FeedbackEntryDto>(x => x.UpdateDate);

                // Make the call to the database
                return scope.Database.Page<FeedbackEntryDto>(page, limit, sql, out total);

            }

        }

        /// <summary>
        /// Gets the entry with the specified <paramref name="entryId"/>.
        /// </summary>
        /// <param name="entryId">The ID of the entry.</param>
        /// <returns>An instance of <see cref="FeedbackEntryDto"/> or <c>null</c> if not found.</returns>
        public FeedbackEntryDto GetEntryById(int entryId) {

            using (var scope = _scopeProvider.CreateScope()) {

                // Declare the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<FeedbackEntryDto>()
                    .From<FeedbackEntryDto>()
                    .Where<FeedbackEntryDto>(x => x.Id == entryId && !x.IsArchived);

                // Make the call to the database
                return scope.Database.First<FeedbackEntryDto>(sql);

            }

        }

        public FeedbackEntryDto GetEntryByKey(Guid key) {

            using (var scope = _scopeProvider.CreateScope()) {

                // Declare the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<FeedbackEntryDto>()
                    .From<FeedbackEntryDto>()
                    .Where<FeedbackEntryDto>(x => x.Key == key && !x.IsArchived);

                // Make the call to the database
                return scope.Database.FirstOrDefault<FeedbackEntryDto>(sql);

            }

        }

        public void Insert(FeedbackEntryDto entry) {
            using (var scope = _scopeProvider.CreateScope()) {
                entry.Id = (int)(decimal)scope.Database.Insert(entry);
                scope.Complete();
            }
        }

        public void Update(FeedbackEntryDto entry) {
            using (var scope = _scopeProvider.CreateScope()) {
                scope.Database.Update(entry);
                scope.Complete();
            }
        }

        public void Delete(FeedbackEntryDto entry) {
            using (var scope = _scopeProvider.CreateScope()) {
                scope.Database.Delete(entry);
                scope.Complete();
            }
        }

        /// <summary>
        /// Deletes all entries before the specified <paramref name="date"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The amount of affected/deleted rows.</returns>
        public int DeleteAll(DateTime date) {

            int affected;

            using (var scope = _scopeProvider.CreateScope()) {

                // Delete everything before the start of the day after "date"
                Sql sql = new Sql($"DELETE FROM {FeedbackConstants.TableName} WHERE Created < '{date.Date.AddDays(1):yyyy-MM-dd}';");

                // Make the call to the database
                affected = scope.Database.Execute(sql);

                scope.Complete();

            }

            return affected;

        }

        #endregion

    }

}
