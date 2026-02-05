using Synapse.Shared.Extensions;

namespace Synapse.HttpClientCls.Core
{
    public class ClsHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public ClsHeader(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public static class ClsHeaderExtensions
    {
        public static List<ClsHeader> ToClsHeaders(this object source)
        {
            var headers = source.ToDictionary().Select(kvp => new ClsHeader(kvp.Key.Replace('_', '-'), (string)kvp.Value)).ToList();
            return headers;
        }
    }
}
