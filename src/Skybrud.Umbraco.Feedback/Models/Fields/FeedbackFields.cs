namespace Skybrud.Umbraco.Feedback.Models.Fields {

    /// <summary>
    /// Class describing the fields of a feedback site.
    /// </summary>
    public class FeedbackFields {

        /// <summary>
        /// Gets or sets the required type of the email field.
        /// </summary>
        public FeedbackFieldType Email { get; set; }

        /// <summary>
        /// Gets or sets the required type of the name field.
        /// </summary>
        public FeedbackFieldType Name { get; set; }

        /// <summary>
        /// Gets or sets the required type of the comment field.
        /// </summary>
        public FeedbackFieldType Comment { get; set; }

    }

}