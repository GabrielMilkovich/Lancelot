using Resurrection.Core.Core.Domain.Entities;
using Resurrection.Domain.Interfaces;

namespace Resurrection.Domain.Services
{
    public class ArchiveService : IArchiveService
    {
        public void EscreverInformacoesArquivo(Arquivo arquivo, LineColor lineColor)
            => File.AppendAllText(arquivo.Caminho, lineColor.Text + Environment.NewLine);

        public void ExcluiInformacoesArquivo(Arquivo arquivo)
            => File.Delete(arquivo.Caminho);
    }
}
