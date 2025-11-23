using System;
using System.Collections.Generic;
using System.Linq;

namespace CoViDAccountant.Components
{
    public static class Attrs
    {
        public static Dictionary<string, object> Empty = new Dictionary<string, object>();
		public static Dictionary<string, object> Multiple = new Dictionary<string, object>() { { "multiple", "" } };
		public static Dictionary<string, object> Hidden = new Dictionary<string, object>() { { "hidden", "hidden" } };
        public static Dictionary<string, object> Disabled = new Dictionary<string, object>() { { "disabled", "disabled" } };
        public static Dictionary<string, object> ReadOnly = new Dictionary<string, object>() { { "readonly", "" } };
		public static Dictionary<string, object> Checked = new Dictionary<string, object>() { { "checked", "" } };
		public static Dictionary<string, object> Selected = new Dictionary<string, object>() { { "selected", "selected" } };
		public static Dictionary<string, object> Rtl = new Dictionary<string, object>() { { "dir", "rtl" } };
        public static Func<string, Dictionary<string, object>> Href = href => new Dictionary<string, object>() { { "href", href } };
		public static Func<string, Dictionary<string, object>> Title = title => new Dictionary<string, object>() { { "title", title } };

        public static Dictionary<string, object> Merge(params Dictionary<string, object>[] attrs)
        {
            return attrs.Where(dict => dict != null)
                        .SelectMany(dict => dict)
                        .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
