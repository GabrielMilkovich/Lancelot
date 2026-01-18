using System.Net;
using System.Net.Http.Headers;

namespace Core.Core.Domain.Entities
{
    public class HttpRequest_Ressurrection
    {
        public HttpRequest_Ressurrection(HttpRequestMessage requestMessage, Dictionary<string, string> headers, object content = null)
        {
            RequestMessage = requestMessage;
            AtribuiHeaders(headers);
            AtribuiContent(content);
        }

        public HttpRequestMessage RequestMessage { get; private set; }
        public HttpRequestHeaders RequestHeaders { get; private set; } = new HttpRequestMessage().Headers;
        public HttpContent RequestContent { get; private set; }

        public void AtribuiHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                RequestMessage.Headers.Add(header.Key, header.Value);
                RequestHeaders.Add(header.Key, header.Value);
            }
        }

        private void AtribuiContent(object content)
        {
            if (content is null)
                return;

            if (content is StringContent stringContent)
            {
                RequestContent = stringContent;
                RequestMessage.Content = stringContent;
            }

            if (content is FormUrlEncodedContent dicContent)
            {
                RequestContent = dicContent;
                RequestMessage.Content = dicContent;
            }
        }
    }
}