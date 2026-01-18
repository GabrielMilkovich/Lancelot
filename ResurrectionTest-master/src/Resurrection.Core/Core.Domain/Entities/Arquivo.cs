using Core;

namespace Resurrection.Core.Core.Domain.Entities
{
    public class Arquivo : BaseEntity
    {
        public Arquivo(string nomeArquivo, string extensão, string caminho = null)
        {
            NomeArquivo = nomeArquivo;
            Extensão = extensão;
            Caminho = string.IsNullOrWhiteSpace(caminho) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{nomeArquivo}.{extensão}") : caminho;

            RealizaValidacoes();
        }

        public string NomeArquivo { get; private set; }
        public string Extensão { get; private set; }
        public string Caminho { get; private set; }

        public override void RealizaValidacoes()
        {
            if (!File.Exists(Caminho))
                AdicionaNotificacao(new ValidationError($"{NomeArquivo} não foi criado na aplicação."));
        }
    }
}
