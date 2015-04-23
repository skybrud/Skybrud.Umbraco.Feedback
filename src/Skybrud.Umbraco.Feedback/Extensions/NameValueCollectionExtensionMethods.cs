using System;
using System.Collections.Specialized;
using System.Linq;

namespace Skybrud.Umbraco.Feedback.Extensions {

    public static class NameValueCollectionExtensionMethods {

        public static int GetInt32(this NameValueCollection nvc, string name) {
            int value;
            return Int32.TryParse(nvc[name], out value) ? value : 0;
        }

        public static int GetInt32(this NameValueCollection nvc, string name, int defaultValue) {
            int value;
            return Int32.TryParse(nvc[name], out value) ? value : defaultValue;
        }

        public static bool ContainsKey(this NameValueCollection nvc, string name) {
            return nvc.AllKeys.Any(x => x == name);
        }

    }

}