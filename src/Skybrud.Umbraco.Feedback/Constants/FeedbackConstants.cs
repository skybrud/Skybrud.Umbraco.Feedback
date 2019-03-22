namespace Skybrud.Umbraco.Feedback.Constants {

    public class FeedbackConstants {

        public const string TableName = "SkybrudFeedback";

        public static class ErrorCodes {

            public const string CreateTableFailed = "228e6515-ea15-4816-865e-fcbb5c6e94de";

            public const string InsertEntryFailed = "306765d1-71e8-470a-87f8-a2e355eb5519";
        
        }

        public static class ErrorMessages {

            public const string DefaultSubmitError = "Your feedback could not be submitted due to an error on the server.";

        }

    }

}