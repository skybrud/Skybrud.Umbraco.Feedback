using System;

namespace Skybrud.Umbraco.Feedback {

    public class FeedbackException : Exception {

        public FeedbackException(string message) : base(message) { }

    }

}