using Microsoft.Extensions.Logging;
using Skybrud.Umbraco.Feedback.Constants;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Feedback.Migrations {

    public class FixEmptyStringValuesMigration : MigrationBase {

        public FixEmptyStringValuesMigration(IMigrationContext context) : base(context) { }

        protected override void Migrate() {

            if (!TableExists(FeedbackConstants.TableName)) return;

            int affected1 = Context.Database.Execute("UPDATE [SkybrudFeedback] SET [Name] = null WHERE [Name] LIKE '';");
            int affected2 = Context.Database.Execute("UPDATE [SkybrudFeedback] SET [Email] = null WHERE [Email] LIKE '';");
            int affected3 = Context.Database.Execute("UPDATE [SkybrudFeedback] SET [Comment] = null WHERE [Comment] LIKE '';");

            Logger.LogInformation($"Fixed empty string values for name column in {affected1} rows.");
            Logger.LogInformation($"Fixed empty string values for email column in {affected2} rows.");
            Logger.LogInformation($"Fixed empty string values for comment column in {affected3} rows.");

        }

    }

}