using Core.Core.Domain.Entities;

namespace Resurrection.Domain.Interfaces
{
    public interface IHttpClient_RessurrectionService
    {
        Task<HttpResponse_Ressurrection> RealizaRequisicao(HttpRequest_Ressurrection request);
        Task<T> RealizaRequisicaoERetornaClasseDeserializada<T>(HttpRequest_Ressurrection request);
        void DiscardSession();
    }
}
