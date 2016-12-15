using System;

namespace Skybrud.Umbraco.Feedback.Exceptions {

    /// <summary>
    /// Class representing an exception thrown by the feedback module.
    /// </summary>
    public class FeedbackException : Exception {

        #region Properties

        /// <summary>
        /// Gets the error code (typically a GUID) identifying the specific error.
        /// </summary>
        public string Code { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new exception with the specified error <code>code</code>.
        /// </summary>
        /// <param name="code">The error code of the exception.</param>
        public FeedbackException(string code) : base("") {
            Code = code;
        }

        /// <summary>
        /// Initializes a new exception with the specified error <code>code</code> and <code>message</code>.
        /// </summary>
        /// <param name="code">The error code of the exception.</param>
        /// <param name="message">The error message of the exception.</param>
        public FeedbackException(string code, string message) : base(message) {
            Code = code;
        }

        #endregion

    }

}