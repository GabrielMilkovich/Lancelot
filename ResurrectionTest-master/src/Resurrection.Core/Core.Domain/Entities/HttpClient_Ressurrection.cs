using RandomUserAgent;
using System;
using System.Net;

namespace Core.Core.Domain.Entities
{
    public class HttpClientRessurrection : IDisposable
    {
        private readonly HttpClientHandler _handler;
        private bool _disposed;

        public HttpClient Client { get; private set; }
        public CookieContainer Cookies { get; }

        public HttpClientRessurrection(string userAgent)
        {
            Cookies = new CookieContainer();

            var handler = new HttpClientHandler
            {
                CookieContainer = Cookies,
                UseCookies = true,
                AllowAutoRedirect = false,
            };

            Client = new HttpClient(handler)
            {
                //Timeout = TimeSpan.FromSeconds(15)
            };

            Client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        }

        public void Dispose()
        {
            if (_disposed) return;

            Client?.Dispose();
            _handler?.Dispose();

            Client = null;

            _disposed = true;
        }
    }
}
