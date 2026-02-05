namespace Synapse.HttpClientCls.Core
{
    public class ParameterCollection : Dictionary<string, object>
    {
        public ParameterCollection()
        {
        }

        public static ParameterCollection GetNameFilter(string value) => new ParameterCollection { { "nameFilter", value } };

        public ParameterCollection Add(string key, object value)
        {
            if (!base.ContainsKey(key))
            {
                base.Add(key, value?.ToString());
            }
            return this;
        }

        public ParameterCollection AddOrUpdate(string key, object value)
        {
            if (!base.ContainsKey(key))
            {
                base.Add(key, value?.ToString());
            }
            else
            {
                base[key] = value?.ToString();
            }
            return this;
        }

        public override string ToString()
        {
            return string.Join(", ", base.Keys.Where(k => !k.StartsWith("__Request")).Select(k => $"[{k}:{base[k].ToString()}]"));
        }
    }
}
