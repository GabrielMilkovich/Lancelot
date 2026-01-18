
using Resurrection.Core.Core.Domain.Entities;

namespace Resurrection.Domain.Interfaces
{
    public interface IArchiveService
    {
        void EscreverInformacoesArquivo(Arquivo arquivo, LineColor lineColor);
        void ExcluiInformacoesArquivo(Arquivo arquivo);
    }
}
