using Core.Core.Domain.Base;
using Core.Core.Domain.Entities;
using Resurrection.Domain.Interfaces;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Resurrection.Domain.Services
{
    public class HttpClient_RessurrectionService :
        RessurrectionBase,
        IHttpClient_RessurrectionService
    {
        public HttpClient_RessurrectionService(HttpClientRessurrection client) : base(client)
        {
        }

        public async Task<HttpResponse_Ressurrection> RealizaRequisicao(HttpRequest_Ressurrection request)
        {
            var response = await Http.Client.SendAsync(request.RequestMessage);

            var uri = request.RequestMessage.RequestUri;
            var cookie = Http.Cookies.GetCookieHeader(uri);
            string content;

            if (response.Content.Headers.ContentEncoding.Any(e => e.Equals("gzip", StringComparison.OrdinalIgnoreCase)))
            {
                var gzip = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
                var reader = new StreamReader(gzip, Encoding.UTF8);

                content = await reader.ReadToEndAsync();
            }
            else
                content = await response.Content.ReadAsStringAsync();


            return new HttpResponse_Ressurrection(
                response,
                response.Headers,
                content,
                cookie);
        }

        public async Task<T> RealizaRequisicaoERetornaClasseDeserializada<T>(HttpRequest_Ressurrection request)
        {
            var envioRequest = await Http.Client.SendAsync(request.RequestMessage);

            if (envioRequest.Content.Headers.ContentEncoding.Any(e => e.Equals("gzip", StringComparison.OrdinalIgnoreCase)))
            {
                var gzip = new GZipStream(await envioRequest.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
                var reader = new StreamReader(gzip, Encoding.UTF8);

                string json1 = await reader.ReadToEndAsync();

                var test = JsonSerializer.Deserialize<T>(json1);

                return JsonSerializer.Deserialize<T>(json1);
            }

            var json = await envioRequest.Content.ReadFromJsonAsync<T>();

            return json;
        }

        public void DiscardSession()
        => Http.Dispose();
    }
}