using System;
using Skybrud.Umbraco.Feedback.Model;

// ReSharper disable MemberHidesStaticFromOuterClass

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

        public static class Ratings {

            public static class Keys {

                public static readonly Guid Positive = new Guid("ab6a18cd-1099-4a6d-87fe-41c67c4c68b0");

                public static readonly Guid Negative = new Guid("a0b91e26-72ac-4baf-bf6d-51f59f1261ea");

            }

            public static readonly FeedbackRating Positive = new FeedbackRating(Keys.Positive, "positive");

            public static readonly FeedbackRating Negative = new FeedbackRating(Keys.Negative, "negative");

        }

        public static class Statuses {

            public static class Keys {

                public static readonly Guid New = new Guid("9fd0bed5-d258-4394-81e7-0ed9b0f1f897");

                public static readonly Guid InProgress = new Guid("be04c7f3-aaad-413c-be14-0b92cf3747cf");

                public static readonly Guid Closed = new Guid("0ef70799-484b-46a9-9699-0863bac43863");

            }

            public static readonly FeedbackStatus New = new FeedbackStatus(Keys.New, "new");

            public static readonly FeedbackStatus InProgress = new FeedbackStatus(Keys.InProgress, "inprogress");

            public static readonly FeedbackStatus Closed = new FeedbackStatus(Keys.Closed, "closed");

        }

    }

}