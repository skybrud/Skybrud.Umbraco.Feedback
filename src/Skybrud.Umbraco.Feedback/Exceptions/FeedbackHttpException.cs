using System;
using System.Net;

namespace Skybrud.Umbraco.Feedback.Exceptions {

    /// <summary>
    /// Class representing an exception thrown by an API in the feedback module.
    /// </summary>
    public class FeedbackHttpException : Exception {

        #region Properties

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode Code { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new exception with the specified error <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The error message of the exception.</param>
        public FeedbackHttpException(string message) : base(message) {
            Code = HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Initializes a new exception with the specified error <paramref name="code"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="code">The error code of the exception.</param>
        /// <param name="message">The error message of the exception.</param>
        public FeedbackHttpException(HttpStatusCode code, string message) : base(message) {
            Code = code;
        }

        #endregion

    }

}