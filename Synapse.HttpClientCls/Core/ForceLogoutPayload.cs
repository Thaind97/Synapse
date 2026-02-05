namespace Synapse.HttpClientCls.Core
{
    public class ForceLogoutPayload
    {
        public bool SkipAlert;
        public string Reason;
        public ForceLogoutPayload(bool skipAlert = false, string reason = null)
        {
            SkipAlert = skipAlert;
            Reason = reason;
        }
    }
}