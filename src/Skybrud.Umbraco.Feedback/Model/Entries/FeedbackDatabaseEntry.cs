using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Skybrud.Umbraco.Feedback.Model.Entries {

    [TableName("SkybrudFeedback")]
    [PrimaryKey("Id", autoIncrement = true)]
    [ExplicitColumns]
    class FeedbackDatabaseEntry {

        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("UniqueId")]
        public string UniqueId { get; set; }

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

        [Column("Updated")]
        public DateTime Updated { get; set; }

        [Column("AssignedTo")]
        public int AssignedTo { get; set; }

        [Column("Archived")]
        public bool IsArchived { get; set; }
    
    }

}