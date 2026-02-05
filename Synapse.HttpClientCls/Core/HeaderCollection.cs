namespace Synapse.HttpClientCls.Core
{
    public class HeaderCollection : List<ClsHeader>
    {
        public HeaderCollection()
        {
        }

        public HeaderCollection(IEnumerable<ClsHeader> headers) : base(headers)
        {
        }
    }
}
