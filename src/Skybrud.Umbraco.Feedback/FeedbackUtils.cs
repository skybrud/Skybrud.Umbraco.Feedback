#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback {

    public static class FeedbackUtils {

        public static string TrimToNull(string str) {
            if (str == null) {
                return null;
            }

            str = str.Trim();
            return str == string.Empty ? null : str;
        }

    }

}