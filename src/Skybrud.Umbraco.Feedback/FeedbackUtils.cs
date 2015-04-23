using System.Collections.Generic;
using System.Linq;
using Skybrud.Umbraco.Feedback.Interfaces;
using Skybrud.Umbraco.Feedback.Model;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Skybrud.Umbraco.Feedback {

    public static class FeedbackUtils {

        static UmbracoDatabase Database {
            get { return ApplicationContext.Current.DatabaseContext.Database; }
        }

        public static string TrimToNull(string str) {
            if (str == null) return null;
            str = str.Trim();
            return str == "" ? null : str;
        }

        public static FeedbackEntry[] GetAll() {

            // Check if the DB table does NOT exist
            if (!Database.TableExist("SkybrudFeedback")) {

                // Create DB table - and set overwrite to false
                Database.CreateTable<FeedbackDatabaseEntry>(false);

            }
            
            // Call this to make sure the users have been loaded before quering the database (otherwise we might exceptions depending on the database provider)
            Dictionary<int, IFeedbackUser> users = FeedbackModule.Instance.GetUsers();

            return (
                from entry in Database.Query<FeedbackDatabaseEntry>(new Sql().Select("*").From("SkybrudFeedback").OrderBy("created DESC"))
                select new FeedbackEntry(entry, users)
            ).ToArray();

        }

        public static FeedbackEntry[] GetAllForSite(int siteId) {

            //Check if the DB table does NOT exist
            if (!Database.TableExist("SkybrudFeedback")) {

                //Create DB table - and set overwrite to false
                Database.CreateTable<FeedbackDatabaseEntry>(false);

            }

            // Call this to make sure the users have been loaded before quering the database (otherwise we might exceptions depending on the database provider)
            Dictionary<int, IFeedbackUser> users = FeedbackModule.Instance.GetUsers();

            // Make sure to convert to an array or similar here so the database is queried immidiately (otherwise we might exceptions depending on the database provider)
            FeedbackDatabaseEntry[] entries = Database.Query<FeedbackDatabaseEntry>(new Sql().Select("*").From("SkybrudFeedback").Where<FeedbackDatabaseEntry>(x => x.SiteId == siteId && !x.IsArchived).OrderBy("created DESC")).ToArray();

            return (
                from entry in entries
                select new FeedbackEntry(entry, users)
            ).ToArray();

        }

        public static FeedbackEntry GetFromId(int entryId) {

            //Check if the DB table does NOT exist
            if (!Database.TableExist("SkybrudFeedback")) {

                //Create DB table - and set overwrite to false
                Database.CreateTable<FeedbackDatabaseEntry>(false);

            }

            // Call this to make sure the users have been loaded before quering the database (otherwise we might exceptions depending on the database provider)
            Dictionary<int, IFeedbackUser> users = FeedbackModule.Instance.GetUsers();
            
            FeedbackDatabaseEntry row = Database.First<FeedbackDatabaseEntry>(new Sql().Select("*").From("SkybrudFeedback").Where<FeedbackDatabaseEntry>(x => x.Id == entryId && !x.IsArchived));

            return row == null ? null : new FeedbackEntry(row, users);

        }

    }

}