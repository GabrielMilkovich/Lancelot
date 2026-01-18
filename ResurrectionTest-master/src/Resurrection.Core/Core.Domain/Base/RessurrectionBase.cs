using Core.Core.Domain.Entities;

namespace Core.Core.Domain.Base
{
    public abstract class RessurrectionBase
    {
        protected HttpClientRessurrection Http { get; private set; }

        protected RessurrectionBase(HttpClientRessurrection client)
        {
            Http = client;
        }

        protected void EnsureNotDisposed()
        {
            if (Http == null)
                throw new ObjectDisposedException(nameof(HttpClientRessurrection));
        }
    }
}