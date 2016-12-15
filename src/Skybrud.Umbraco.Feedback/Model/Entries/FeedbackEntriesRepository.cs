using System;
using System.Collections.Generic;
using Skybrud.Umbraco.Feedback.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Skybrud.Umbraco.Feedback.Model.Entries {
    
    public class FeedbackEntriesRepository {

        #region Properties
        
        protected UmbracoDatabase Database {
            get { return ApplicationContext.Current.DatabaseContext.Database; }
        }

        protected DatabaseSchemaHelper DatabaseHelper {
            get {
                ILogger logger = LoggerResolver.Current.Logger;
                DatabaseContext dbContext = ApplicationContext.Current.DatabaseContext;
                return new DatabaseSchemaHelper(dbContext.Database, logger, dbContext.SqlSyntax);
            }
        }

        /// <summary>
        /// Gets a reference to the parent feedback module.
        /// </summary>
        protected FeedbackModule Feedback { get; private set; }

        #endregion

        #region Constructors

        internal FeedbackEntriesRepository(FeedbackModule feedback) {
            Feedback = feedback;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Gets the entry with the specified <paramref name="uniqueId"/>, or <code>null</code> if not found.
        /// </summary>
        /// <param name="uniqueId">The unique ID of the entry.</param>
        /// <returns></returns>
        public FeedbackEntry GetEntryById(string uniqueId) {

            // Return NULL if the specified "uniqueId" is empty
            if (String.IsNullOrWhiteSpace(uniqueId)) return null;

            // We simply return NULL if the table doesn't exists (since there aren't any entries anyway)
            if (!DatabaseHelper.TableExist("SkybrudFeedback")) return null;

            // Call this to make sure the users have been loaded before quering the database (otherwise we might exceptions depending on the database provider)
            Dictionary<int, IFeedbackUser> users = FeedbackModule.Instance.GetUsers();

            // Generate the SQL for the query
            Sql sql = new Sql("SELECT * FROM SkybrudFeedback WHERE UniqueId = @0;", uniqueId);

            // Make the call to the database
            FeedbackDatabaseEntry first = Database.FirstOrDefault<FeedbackDatabaseEntry>(sql);

            // Wrap the database entry in an instance of "FeedbackEntry"
            return first == null ? null : new FeedbackEntry(first, users);

        }
        
        public bool Save(FeedbackEntry entry) {
            
            if (entry == null) throw new ArgumentNullException("entry");

            // Trigger the "OnEntryUpdating" before assigning the user
            foreach (IFeedbackPlugin plugin in Feedback.Plugins) {
                try {
                    if (!plugin.OnEntryUpdating(Feedback, entry)) {
                        return false;
                    }
                } catch (Exception ex) {
                    LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnEntryUpdating.", ex);
                }
            }

            ApplicationContext.Current.DatabaseContext.Database.Update(entry.Row);

            // Trigger the "OnEntryUpdated" event when the user has been assigned
            foreach (IFeedbackPlugin plugin in Feedback.Plugins) {
                try {
                    plugin.OnEntryUpdated(Feedback, entry);
                } catch (Exception ex) {
                    LogHelper.Error<FeedbackModule>("Plugin of type " + plugin.GetType() + " failed for method OnEntryUpdated.", ex);
                }
            }

            return true;
        
        }

        #endregion

    }

}