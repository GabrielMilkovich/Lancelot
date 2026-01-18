using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Core.Core.Domain.Entities
{
    public class HttpResponse_Ressurrection
    {
        public HttpResponse_Ressurrection(HttpResponseMessage responseMessage, HttpResponseHeaders responseHeader, string content, string cookie)
        {
            ResponseMessage = responseMessage;
            ResponseHeader = responseHeader;
            ContentRead = content;
            Cookie = cookie;
            StatusCode = (int)ResponseMessage.StatusCode;
        }

        public int StatusCode { get; private set; }
        public HttpResponseMessage ResponseMessage { get; private set; }
        public HttpResponseHeaders ResponseHeader { get; private set; }
        public string ContentRead { get; private set; }
        public string Cookie { get; private set; }
    }
}
