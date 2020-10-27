using System;
using NPoco;
using Skybrud.Umbraco.Feedback.Constants;
using Skybrud.Umbraco.Feedback.Model.Entries;
using Skybrud.Umbraco.Feedback.Plugins;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Scoping;

namespace Skybrud.Umbraco.Feedback.Services {
    
    public class FeedbackDatabaseService {

        private readonly IScopeProvider _scopeProvider;
        
        private readonly ILogger _logger;
        
        #region Properties

        protected FeedbackPluginCollection Plugins => FeedbackPluginCollection.Current;

        #endregion

        #region Constructors

        public FeedbackDatabaseService(IScopeProvider scopeProvider, ILogger logger) {
            _scopeProvider = scopeProvider;
            _logger = logger;
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
                return scope.Database.First<FeedbackEntryDto>(sql);

            }

        }

        public void Insert(FeedbackEntryDto entry) {
            using (var scope = _scopeProvider.CreateScope()) {
                entry.Id = (int) (decimal) scope.Database.Insert(entry);
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
