using Skybrud.Umbraco.Feedback.Model.Entries;
using Skybrud.Umbraco.Feedback.Models;
using Skybrud.Umbraco.Feedback.Services;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Feedback {

    public static class FeedbackUtils {

        public static string TrimToNull(string str) {
            if (str == null) return null;
            str = str.Trim();
            return str == "" ? null : str;
        }

        //public static FeedbackEntry[] GetAll() {
        //    return new FeedbackService().GetAll();
        //}

        //public static FeedbackEntry[] GetAllForSite(int siteId) {
        //    return new FeedbackService().GetAllForSite(siteId);
        //}

        //public static FeedbackEntry GetFromId(int entryId) {
        //    return new FeedbackService().GetEntryById(entryId);
        //}

    }

}