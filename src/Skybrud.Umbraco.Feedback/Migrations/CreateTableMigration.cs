using Skybrud.Umbraco.Feedback.Constants;
using Skybrud.Umbraco.Feedback.Models.Entries;
using Umbraco.Core.Migrations;

namespace Skybrud.Umbraco.Feedback.Migrations {
    
    public class CreateTableMigration : MigrationBase {

        public CreateTableMigration(IMigrationContext context) : base(context) { }

        public override void Migrate() {
            if (TableExists(FeedbackConstants.TableName)) return;
            Create.Table<FeedbackEntrySchema>().Do();
        }

    }

}