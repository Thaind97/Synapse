using HtmlAgilityPack;
using Synapse.Shared.Extensions;

namespace Synapse.HttpClientCls.Core
{
    public interface IClsHtmlDocument
    {
        IEnumerable<HtmlNode> Select(string xpath);
    }

    public class ClsHtmlDocument : IClsHtmlDocument
    {
        private HtmlDocument doc;

        public ClsHtmlDocument(string htmlContent)
        {
            doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
        }

        public IEnumerable<HtmlNode> Select(string xpath)
        {
            return doc.DocumentNode.SelectNodes(xpath).EmptyIfNull();
        }
    }
}
