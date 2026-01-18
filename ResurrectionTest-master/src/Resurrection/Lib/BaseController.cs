using Autofac;
using Core;
using Core.Core.Domain.Entities;
using DarkPassenger.Domain.Entities;
using Resurrection.Configurations;
using Resurrection.Core.Core.Domain.Entities;
using Resurrection.Domain.Entities;
using Resurrection.Domain.Enum;
using Resurrection.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text;
using System.Web;

namespace Resurrection.Lib
{
    public abstract class BaseController
    {
        public readonly IHttpClient_RessurrectionService _httpClient_RessurrectionService;
        private readonly ILifetimeScope _currentScope;
        private readonly IArchiveService _archiveService;

        private HttpResponse_Ressurrection LastResponse;
        private HttpRequest_Ressurrection LastRequest;

        private List<HttpResponse_Ressurrection> AllResponses = new List<HttpResponse_Ressurrection>();
        private List<HttpRequest_Ressurrection> AllRequests = new List<HttpRequest_Ressurrection>();

        public Account Account { get; private set; }

        public BaseController()
        {
            _currentScope = AppParams.Container.BeginLifetimeScope();
            _httpClient_RessurrectionService = _currentScope.Resolve<IHttpClient_RessurrectionService>();
            _archiveService = _currentScope.Resolve<IArchiveService>();
        }

        public abstract Task StartProcess(Account account);

        public abstract Task<EnriqueceResult> EnriqueceChecker(Login login);

        public abstract ValidationError ImprimeInformacoesNaTela(Dictionary<Account, Checker> accountRica);

        public async Task RealizaRequisicao(HttpRequest_Ressurrection request)
        {
            var response = await _httpClient_RessurrectionService.RealizaRequisicao(request);

            LastResponse = response;
            LastRequest = request;

            AllResponses.Add(response);
            AllRequests.Add(request);
        }

        public async Task<T> RequisicaoDeserializadaJson<T>(HttpRequest_Ressurrection request)
            => await _httpClient_RessurrectionService.RealizaRequisicaoERetornaClasseDeserializada<T>(request);

        public void AtribuiAccount(Account account)
            => Account = account;

        public HttpRequest_Ressurrection GetLastRequest()
            => LastRequest;

        public HttpResponse_Ressurrection GetLastResponse()
            => LastResponse;

        public List<HttpResponse_Ressurrection> GetAllResponses()
            => AllResponses;

        public List<HttpRequest_Ressurrection> GetAllRequests()
            => AllRequests;

        public async Task<Uri> BuscaLocationRequisicoes(HttpRequest_Ressurrection httpRequest)
        {
            await RealizaRequisicao(httpRequest);
            return LastResponse.ResponseHeader.Location;
        }

        public Uri BuscaLocationUltimaRequisicao()
        {
            if (LastResponse.ResponseHeader.Location is null)
                return null;

            return LastResponse.ResponseHeader.Location;
        }

        public StringContent RetornaContentTratada(string data, Encoding encoding)
        {
            if (encoding == Encoding.UTF8)
                return new StringContent(data, encoding, "application/x-www-form-urlencoded");

            return new StringContent(data, encoding);
        }

        public Dictionary<string, string> AtribuiCampoEmUmPadraoExistente(Dictionary<string, string> padrao, Dictionary<string, string> personalizado)
        {
            var retorno = new Dictionary<string, string>();

            foreach (var item in padrao)
                retorno.Add(item.Key, item.Value);

            foreach (var key in personalizado)
                retorno.Add(key.Key, key.Value);

            return retorno;
        }

        public string RetornaUrlComParametros(Dictionary<string, string> parametros, string url)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in parametros)
                query[param.Key] = param.Value;

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri.ToString();
        }

        public void ConsoleLogColor(LineColor lineColor)
        {
            Console.ForegroundColor = lineColor.Color;
            Console.WriteLine(lineColor.Text + (lineColor.PulaLinha ? Environment.NewLine : string.Empty));
            Console.ResetColor();
        }

        public void InsereEmArquivoTxt(Arquivo arquivo, LineColor lineColor)
            => _archiveService.EscreverInformacoesArquivo(arquivo, lineColor);
    }
}
