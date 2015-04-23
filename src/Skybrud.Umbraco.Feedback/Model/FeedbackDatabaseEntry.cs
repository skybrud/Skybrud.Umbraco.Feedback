using System;
using Skybrud.Umbraco.Feedback.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Web;

namespace Skybrud.Umbraco.Feedback.Model {

    [TableName("SkybrudFeedback")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    public class FeedbackDatabaseEntry {

        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("SiteId")]
        public int SiteId { get; set; }

        [Column("PageId")]
        public int PageId { get; set; }

        [Column("Name")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Name { get; set; }

        [Column("Email")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Email { get; set; }

        [Column("Rating")]
        public string Rating { get; set; }

        [Column("Comment")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Comment { get; set; }

        [Column("Status")]
        public string Status { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }

        [Column("AssignedTo")]
        public int AssignedTo { get; set; }

        [Column("Archived")]
        public bool IsArchived { get; set; }

        public void ChangeStatus(FeedbackStatus status) {
            Status = status.Alias;
            UmbracoDatabase db = UmbracoContext.Current.Application.DatabaseContext.Database;
            db.Update("SkybrudFeedback", "Id", this);
        }

        public void SetAssignedTo(IFeedbackUser user) {
            AssignedTo = (user == null ? 0 : user.Id);
            UmbracoDatabase db = UmbracoContext.Current.Application.DatabaseContext.Database;
            db.Update("SkybrudFeedback", "Id", this);
        }

        public void Archive() {
            IsArchived = true;
            UmbracoDatabase db = UmbracoContext.Current.Application.DatabaseContext.Database;
            db.Update("SkybrudFeedback", "Id", this);
        }
    
    }

}