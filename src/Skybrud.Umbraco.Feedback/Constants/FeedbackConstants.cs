using Skybrud.Umbraco.Feedback.Models.Ratings;
using Skybrud.Umbraco.Feedback.Models.Statuses;
using System;

#pragma warning disable 1591

// ReSharper disable MemberHidesStaticFromOuterClass

namespace Skybrud.Umbraco.Feedback.Constants {

    /// <summary>
    /// Static class with various constants related to the feedback package.
    /// </summary>
    public static class FeedbackConstants {

        /// <summary>
        /// Gets the alias of the feedback datatabe table.
        /// </summary>
        public const string TableName = "SkybrudFeedback";
        
        public static class Ratings {

            public static class Keys {

                /// <summary>
                /// Gets the GUID of the default negative rating.
                /// </summary>
                public static readonly Guid Negative = new("00000000-0000-0000-0001-000000001000");
                
                /// <summary>
                /// Gets the GUID of the default positive rating.
                /// </summary>
                public static readonly Guid Positive = new("00000000-0000-0000-0001-000000002000");

            }

            /// <summary>
            /// Gets a reference to the default negative rating.
            /// </summary>
            public static readonly FeedbackRating Negative = new(Keys.Negative, "negative");
            
            /// <summary>
            /// Gets a reference to the default positive rating.
            /// </summary>
            public static readonly FeedbackRating Positive = new(Keys.Positive, "positive");

        }

        public static class Statuses {

            public static class Keys {
                
                /// <summary>
                /// Gets the GUID of the default <strong>New</strong> status.
                /// </summary>
                public static readonly Guid New = new("00000000-0000-0000-0002-000000001000");
                
                /// <summary>
                /// Gets the GUID of the default <strong>In Progress</strong> status.
                /// </summary>
                public static readonly Guid InProgress = new("00000000-0000-0000-0002-000000002000");
                
                /// <summary>
                /// Gets the GUID of the default <strong>Closed</strong> status.
                /// </summary>
                public static readonly Guid Closed = new("00000000-0000-0000-0002-000000003000");

            }
            
            /// <summary>
            /// Gets a reference to the default <strong>New</strong> rating.
            /// </summary>
            public static readonly FeedbackStatus New = new(Keys.New, "new");
            
            /// <summary>
            /// Gets a reference to the default <strong>In Progress</strong> rating.
            /// </summary>
            public static readonly FeedbackStatus InProgress = new(Keys.InProgress, "inprogress");
            
            /// <summary>
            /// Gets a reference to the default <strong>Closed</strong> rating.
            /// </summary>
            public static readonly FeedbackStatus Closed = new(Keys.Closed, "closed");

        }

    }

}