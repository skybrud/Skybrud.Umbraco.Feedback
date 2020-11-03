using System;
using Skybrud.Umbraco.Feedback.Models.Ratings;
using Skybrud.Umbraco.Feedback.Models.Statuses;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace Skybrud.Umbraco.Feedback.Constants {

    public class FeedbackConstants {

        public const string TableName = "SkybrudFeedback";

        public static class Ratings {

            public static class Keys {

                public static readonly Guid Negative = new Guid("00000000-0000-0000-0001-000000001000");

                public static readonly Guid Positive = new Guid("00000000-0000-0000-0001-000000002000");

            }

            public static readonly FeedbackRating Negative = new FeedbackRating(Keys.Negative, "negative");

            public static readonly FeedbackRating Positive = new FeedbackRating(Keys.Positive, "positive");

        }

        public static class Statuses {

            public static class Keys {

                public static readonly Guid New =        new Guid("00000000-0000-0000-0002-000000001000");

                public static readonly Guid InProgress = new Guid("00000000-0000-0000-0002-000000002000");

                public static readonly Guid Closed =     new Guid("00000000-0000-0000-0002-000000003000");

            }

            public static readonly FeedbackStatus New = new FeedbackStatus(Keys.New, "new");

            public static readonly FeedbackStatus InProgress = new FeedbackStatus(Keys.InProgress, "inprogress");

            public static readonly FeedbackStatus Closed = new FeedbackStatus(Keys.Closed, "closed");

        }

    }

}