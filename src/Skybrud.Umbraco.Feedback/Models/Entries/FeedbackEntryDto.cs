using NPoco;
using System;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback.Models.Entries {

    [TableName("SkybrudFeedback")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class FeedbackEntryDto {

        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Key")]
        public Guid Key { get; set; }

        [Column("SiteKey")]
        public Guid SiteKey { get; set; }

        [Column("PagKey")]
        public Guid PageKey { get; set; }

        [Column("Name")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Name { get; set; }

        [Column("Email")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Email { get; set; }

        [Column("Comment")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Comment { get; set; }

        [Column("Rating")]
        public Guid Rating { get; set; }

        [Column("Status")]
        public Guid Status { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("UpdateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("AssignedTo")]
        public Guid AssignedTo { get; set; }

        [Column("Archived")]
        public bool IsArchived { get; set; }

    }

}