using Autofac;
using Core.Core.Domain.Entities;
using Resurrection.Configurations;
using Resurrection.Domain.Constantes;
using Resurrection.Domain.Entities;
using Resurrection.Domain.Interfaces;
using System.IO.Compression;
using System.Text;
using DarkPassenger.Domain.Entities;
using Resurrection.Domain.Enum;
using System.Collections.Concurrent;

namespace Resurrection.App.Controller
{
    public class InicializacaoController
    {
        private readonly IAccountService _accountService;
        private readonly IArchiveService _archiveService;
        private readonly ILifetimeScope _currentScope;


        public InicializacaoController()
        {
            _currentScope = AppParams.Container.BeginLifetimeScope();
            _accountService = _currentScope.Resolve<IAccountService>();
            _archiveService = _currentScope.Resolve<IArchiveService>();
        }

        public async Task StartProcess()
        {
            var accounts = _accountService.BuscaContasBlocoNotas();
            var tasks = new List<Task>();

            var accountsCompleted = new ConcurrentDictionary<Account, Login>();
            using var semaphore = new SemaphoreSlim(1);

            foreach (var account in accounts)
            {

                var task = Task.Run(async () =>
                {
                    try
                    {
                        await semaphore.WaitAsync();
                        var resultado = await new LoginRequest().LogaConta(account);

                        accountsCompleted.TryAdd(account, resultado.Situacao);
                    }
                    finally
                    {
                        semaphore.Release();
                        Console.Title = $@" {accountsCompleted.Count}/{accounts.Count} | contas com erro de sessão: {accountsCompleted.Where(w => w.Value == Login.SessaoBloqueada).ToList().Count}";
                    }
                });
                tasks.Add(task);

            }

            await Task.WhenAll(tasks);

            Console.WriteLine($@"contas zoadas: {accountsCompleted.Where(w => w.Value == Login.SessaoBloqueada).ToList().Count}");

            Console.ReadLine();
        }

        public class LoginRequest
        {
            private string LastContentRead;
            private HttpResponse_Ressurrection LastResponse;

            public LoginRequest()
            {
            }

            public async Task<LoginResult> LogaConta(Account account)
            {
                string randomUserAgent = MorganConstantes.userAgents[new Random().Next(MorganConstantes.userAgents.Count)];

                #region Etapa get login

                var client = new HttpClientRessurrection(randomUserAgent);

                var requestClient = new HttpRequestMessage(HttpMethod.Get, MorganConstantes.Microsoft.PostLoginMicrosoft);
                var headerClient = MorganConstantes.Microsoft.Headers.LoginPostSrc;

                await RealizaRequisicao(client, new HttpRequest_Ressurrection(requestClient, headerClient));

                #endregion Etapa get login

                #region Etapa post login -- Principal

                var responseDatasLogin = new ResponseDatasLogin(LastContentRead, account);


                await RealizaRequisicao(client, new HttpRequest_Ressurrection(
                    new HttpRequestMessage(HttpMethod.Post, responseDatasLogin.LogUrl), MorganConstantes.Microsoft.Headers.PostLogUrl,
                        RetornaContentTratada(responseDatasLogin.Data, Encoding.UTF8)));

                responseDatasLogin.AtribuiErrText(LastContentRead);

                if (LastContentRead.Contains("JavaScript required to sign in"))
                {
                    client.Dispose();
                    return new LoginResult(Domain.Enum.Login.SessaoBloqueada);
                }

                var validacaoAcessoConta = responseDatasLogin.Validacao();


                if (!string.IsNullOrWhiteSpace(validacaoAcessoConta.ErrorMessage) || !string.IsNullOrWhiteSpace(responseDatasLogin.ErrText))
                {
                    return new LoginResult(Domain.Enum.Login.ErroCredenciais);
                }

                return new LoginResult(Domain.Enum.Login.Logado);

                #endregion Etapa post login -- Principal
            }

            private async Task RealizaRequisicao(HttpClientRessurrection httpClient, HttpRequest_Ressurrection httpRequest)
            {
                var response = await httpClient.Client.SendAsync(httpRequest.RequestMessage);

                var uri = httpRequest.RequestMessage.RequestUri;
                var cookie = httpClient.Cookies.GetCookieHeader(uri);

                if (response.Content.Headers.ContentEncoding.Any(e => e.Equals("gzip", StringComparison.OrdinalIgnoreCase)))
                {
                    var gzip = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
                    var reader = new StreamReader(gzip, Encoding.UTF8);

                    LastContentRead = await reader.ReadToEndAsync();
                }
                else
                    LastContentRead = await response.Content.ReadAsStringAsync();

                LastResponse = new HttpResponse_Ressurrection(
                response,
                response.Headers,
                LastContentRead,
                cookie);

            }

            public StringContent RetornaContentTratada(string data, Encoding encoding)
            {
                if (encoding == Encoding.UTF8)
                    return new StringContent(data, encoding, "application/x-www-form-urlencoded");

                return new StringContent(data, encoding);
            }

        }
    }
}