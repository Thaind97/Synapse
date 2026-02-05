using Newtonsoft.Json;

namespace Synapse.HttpClientCls.Core
{
    public class ClsBody
    {
        private IClsHtmlDocument _htmlContent;
        private string _stringContent;

        public string StringContent
        {
            get
            {
                return _stringContent;
            }

            private set
            {
                if (_htmlContent != null)
                {
                    _htmlContent = null;
                }

                _stringContent = value;
            }
        }

        public IClsHtmlDocument HtmlContent
        {
            get
            {
                if (_htmlContent == null)
                {
                    _htmlContent = new ClsHtmlDocument(StringContent);
                }

                return _htmlContent;
            }
        }

        public string FilterByXpath(string xpathExpression, string attributeToRetrive) => HtmlContent.Select(xpathExpression).FirstOrDefault().GetAttributeValue(attributeToRetrive, string.Empty);
        public string FilterByXpathAndGetInnerText(string xpathExpression) => HtmlContent.Select(xpathExpression).FirstOrDefault().InnerText;

        public ClsBody(string content)
        {
            StringContent = content;
        }

        public dynamic AsJson(bool exposeError = false)
        {
            try
            {
                return JsonConvert.DeserializeObject<dynamic>(StringContent);
            }
            catch (Exception)
            {
                if (exposeError)
                {
                    throw;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
