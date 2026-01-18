using Resurrection.Domain.Entities;

namespace Resurrection.Domain.Interfaces
{
    public interface IAccountService
    {
        List<Account> BuscaContasBlocoNotas();
    }
}
