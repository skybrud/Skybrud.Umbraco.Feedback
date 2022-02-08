using Skybrud.Umbraco.Feedback.Constants;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Feedback.Migrations {

    public class CreateTableMigration : MigrationBase {

        public CreateTableMigration(IMigrationContext context) : base(context) { }

        protected override void Migrate() {
            if (TableExists(FeedbackConstants.TableName)) {
                return;
            }

            Create.Table<FeedbackEntrySchema>().Do();
        }

    }

}